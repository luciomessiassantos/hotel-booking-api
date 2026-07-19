using BookingAPI.src.Modules.Booking.Application.Services.Interfaces;
using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Dto;
using BookingAPI.src.Modules.Booking.Domain.Dto.HotelDtos;
using BookingAPI.src.Modules.Booking.Domain.Mappers;
using BookingAPI.src.Modules.Booking.Infrastructure;
using BookingAPI.src.Shared.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace BookingAPI.src.Modules.Booking.Application.Services.Implementations;

public class HotelService(
    BookingDbContext context,
    HybridCache cache,
    ILogger<HotelService> logger
) : IHotelService
{

    const string CACHE_TAG = "hotels";
    const string LIST_CACHE_TAG = "hotels_list";
    public async Task<HotelResponseDto> Create(CreateHotelDto request)
    {
        var entity = new Hotel
        {
            Name = request.Name,
            Address = request.Address,
            Amenities = request.Amenities,
            Rooms = [],
            StarRating = 0
        };

        context.Hotels.Add(entity);
        await context.SaveChangesAsync();

        var hotelDto = HotelMapper.ToDto(entity);

        logger.LogInformation("Invalidating cache for tags: {CACHE_TAG}, name:{Name}.", CACHE_TAG, request.Name);
        await cache.RemoveByTagAsync(LIST_CACHE_TAG);

        await cache.SetAsync(
            $"hotel:{hotelDto.Id}",
            hotelDto,
            tags: [CACHE_TAG]
        );

        return hotelDto;
    }

    public async Task<HotelResponseDto?> GetById(Guid id)
    {
        return await cache.GetOrCreateAsync(
            $"hotel:{id}",
            async ct => await context.Hotels
            .AsNoTracking()
            .Select(h => HotelMapper.ToDto(h))
            .FirstOrDefaultAsync(h => h.Id == id, ct),
            tags: [CACHE_TAG],
            cancellationToken: default
        );

    }

    public async Task<PaginatedResult<HotelResponseDto>> List(HotelQueryDto request)
    {
        var queryable = context.Hotels
            .AsNoTracking()
            .WhereIf(request.Term is not null, e => e.Name.Contains(request.Term!))
            .WhereIf(request.Street is not null, e => e.Address.Street.Contains(request.Street!))
            .WhereIf(request.City is not null, e => e.Address.City.Contains(request.City!))
            .WhereIf(request.Country is not null, e => e.Address.Country.Contains(request.Country!))
            .WhereIf(request.State is not null, e => e.Address.State.Contains(request.State!))
            .WhereIf(request.MaxRating is not null, e => e.StarRating <= request.MaxRating)
            .WhereIf(request.MinRating is not null, e => e.StarRating >= request.MinRating)
            .WhereIf(request.MaxRoomPrice is not null, e => e.Rooms.Any(r => r.PricePerNight <= request.MaxRoomPrice))
            .WhereIf(request.MinRoomPrice is not null, e => e.Rooms.Any(r => r.PricePerNight >= request.MinRoomPrice));

        if (request.Deleted is true)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        // Chave de cache composta de dinâmica: Cada filtro vira um fragmento identificador único
        var cacheKey = $"hotels:list:p:{request.PageNumber}" +
                    $":t:{request.Term ?? "all"}" +
                    $":st:{request.Street ?? "all"}" +
                    $":ct:{request.City ?? "all"}" +
                    $":cn:{request.Country ?? "all"}" +
                    $":s:{request.State ?? "all"}" +
                    $":maxr:{request.MaxRating ?? 0}" +
                    $":minr:{request.MinRating ?? 0}" +
                    $":maxp:{request.MaxRoomPrice ?? 0}" +
                    $":minp:{request.MinRoomPrice ?? 0}" +
                    $":d:{request.Deleted ?? false}";

        return await cache.GetOrCreateAsync(
            cacheKey, // chave composta
            async ct => await queryable
                .ToPaginatedResultAsync(
                    request.PageNumber,
                    h => HotelMapper.ToDto(h),
                    cancellationToken: ct
                ),
            tags: [LIST_CACHE_TAG]
        );
    }

    public async Task<HotelResponseDto> Update(PatchHotelDto request)
    {
        var entity = await context.Hotels
            .FirstOrDefaultAsync(h => h.Id == request.HotelId)
            ?? throw new NullReferenceException($"Hotel de Id = {request.HotelId} não encontrado");

        if (request.Name is not null)
        {
            entity.Name = request.Name;
        }

        if (request.StarRating is not null)
        {
            entity.StarRating = (int) request.StarRating;
        }

        if (request.City is not null)
        {
            entity.Address = entity.Address with { City = request.City };
        }

        if (request.Street is not null)
        {
            entity.Address = entity.Address with { Street = request.Street };
        }

        if (request.Country is not null)
        {
            entity.Address = entity.Address with { Country = request.Country };
        }

        if (request.State is not null)
        {
            entity.Address = entity.Address with { State = request.State };
        }

        if (request.ZipCode is not null)
        {
            entity.Address = entity.Address with { ZipCode = request.ZipCode };
        }

        if (request.Number is not null)
        {
            entity.Address = entity.Address with { Number = request.Number };
        }

        if (request.Amenities is not null)
        {
            entity.Amenities = request.Amenities; 
        }


        await context.SaveChangesAsync();

        var hotelDto = HotelMapper.ToDto(entity);
        
        await cache.RemoveAsync($"hotel:{request.HotelId}");
        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        await cache.SetAsync(
            $"hotel:{entity.Id}",
            hotelDto,
            tags: [CACHE_TAG]
        );

        return hotelDto;
    }

    public async Task Restore(Guid id)
    {
        var entity = await context.Hotels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(h => h.Id == id)
            ?? throw new NullReferenceException($"Hotel de Id = {id} não encontrado");

        entity.IsDeleted = false;

        await context.SaveChangesAsync();

        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        await cache.SetAsync(
            $"hotel:{entity.Id}",
            HotelMapper.ToDto(entity),
            tags: [CACHE_TAG]
        );
    }

    public async Task SoftDelete(Guid id)
    {
        var entity = await context.Hotels
            .FirstOrDefaultAsync(h => h.Id == id)
            ?? throw new NullReferenceException($"Hotel de Id = {id} não encontrado");

        entity.IsDeleted = true;
        await cache.RemoveAsync($"hotel:{entity.Id}");
        await cache.RemoveByTagAsync(LIST_CACHE_TAG);

        await context.SaveChangesAsync();
    }

    public async Task HardDelete(Guid id)
    {
        var entity = await context.Hotels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(h => h.Id == id)
            ?? throw new NullReferenceException($"Hotel de Id = {id} não encontrado");

        context.Hotels.Remove(entity);
        
        await cache.RemoveAsync($"hotel:{id}");
        await cache.RemoveByTagAsync(LIST_CACHE_TAG);

        await context.SaveChangesAsync();
    }

}