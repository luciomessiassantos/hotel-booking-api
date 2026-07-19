using BookingAPI.src.Modules.Booking.Application.Services.Interfaces;
using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Dto.GuestDto;
using BookingAPI.src.Modules.Booking.Domain.Mappers;
using BookingAPI.src.Modules.Booking.Infrastructure;
using BookingAPI.src.Shared.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace BookingAPI.src.Modules.Booking.Application.Services.Implementations;


public class GuestService(
    BookingDbContext context,
    HybridCache cache,
    ILogger<GuestService> logger
) : IGuestService
{
    private const string CACHE_TAG = "guests"; 
    private const string LIST_CACHE_TAG = "guests_list";

    public async Task<GuestResponseDto> Create(CreateGuestDto request)
    {
        logger.LogInformation("Criando Hóspede - Email: {Email}", request.Email);

        var entity = new Guest
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Reservations = []
        };

        context.Guests.Add(entity);
        await context.SaveChangesAsync();

        var guestDto = GuestMapper.ToDto(entity);

        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        await cache.SetAsync(
            $"guest:{guestDto.Id}", 
            guestDto, 
            tags: [CACHE_TAG]
        );

        logger.LogInformation("Hóspede Criado - Id: {Id} - Nome: {FullName}", guestDto.Id, guestDto.FullName);
        return guestDto;
    }

    public async Task<GuestResponseDto?> GetById(Guid id)
    {
        return await cache.GetOrCreateAsync(
            $"guest:{id}",
            async ct => await context.Guests
                .Select(g => GuestMapper.ToDto(g))
                .FirstOrDefaultAsync(g => g.Id == id, ct),
            tags: [CACHE_TAG]
        );
    }

    public async Task<PaginatedResult<GuestResponseDto>> List(ListGuestDto request)
    {
        var queryable = context.Guests.AsNoTracking();

        if (request.Deleted is true)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        queryable = queryable
            .WhereIf(request.Term is not null, g => g.FullName.Contains(request.Term!) || g.Email.Contains(request.Term!));

        var cacheKey = $"guests:list:p:{request.PageNumber}" +
                    $":t:{request.Term ?? "all"}" +
                    $":d:{request.Deleted ?? false}";

        return await cache.GetOrCreateAsync(
            cacheKey,
            async ct => await queryable
                .AsNoTracking()
                .ToPaginatedResultAsync(
                    request.PageNumber, 
                    g => GuestMapper.ToDto(g), 
                    cancellationToken: ct
                ),
            tags: [LIST_CACHE_TAG] 
        );
    }

    public async Task<GuestResponseDto> Update(UpdateGuestDto request)
    {
        logger.LogInformation("Atualizando Hóspede - Id: {Id} - Nome Request: {FullName}", request.GuestId, request.FullName);

        var entity = await context
            .Guests
            .FirstOrDefaultAsync(g => g.Id == request.GuestId)
            ?? throw new NullReferenceException($"Hóspede {request.GuestId} não encontrado");

        if (request.FullName is not null) entity.FullName = request.FullName;
        if (request.Email is not null) entity.Email = request.Email;
        if (request.Phone is not null) entity.Phone = request.Phone;

        await context.SaveChangesAsync();
        var dto = GuestMapper.ToDto(entity);

        await cache.RemoveAsync($"guest:{request.GuestId}");
        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        await cache.SetAsync(
            $"guest:{dto.Id}", 
            dto, 
            tags: [CACHE_TAG]
        );

        logger.LogInformation("Hóspede Atualizado - Id: {Id} - Nome: {FullName}", dto.Id, dto.FullName);
        return dto;
    }

    public async Task SoftDelete(Guid id)
    {
        logger.LogInformation("Soft Delete Hóspede - Id: {Id}", id);

        var entity = await context
            .Guests
            .FirstOrDefaultAsync(g => g.Id == id)
            ?? throw new NullReferenceException($"Hóspede {id} não encontrado");

        entity.IsDeleted = true;
        
        await cache.RemoveAsync($"guest:{id}");
        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        
        await context.SaveChangesAsync();
    }

    public async Task Restore(Guid id)
    {
        logger.LogInformation("Restaurando Hóspede - Id: {Id}", id);

        var entity = await context
            .Guests
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(g => g.Id == id)
            ?? throw new NullReferenceException($"Hóspede {id} não encontrado");

        entity.IsDeleted = false;
        await context.SaveChangesAsync();

        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        await cache.SetAsync(
            $"guest:{entity.Id}", 
            GuestMapper.ToDto(entity), 
            tags: [CACHE_TAG]
        );
    }

    public async Task HardDelete(Guid id)
    {
        logger.LogInformation("Hard Delete Hóspede - Id: {Id}", id);

        var entity = await context
            .Guests
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(g => g.Id == id)
            ?? throw new NullReferenceException($"Hóspede {id} não encontrado");

        context.Guests.Remove(entity);

        await cache.RemoveAsync($"guest:{id}");
        await cache.RemoveByTagAsync(LIST_CACHE_TAG);

        await context.SaveChangesAsync();
    }
}