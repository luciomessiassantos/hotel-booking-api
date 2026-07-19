using BookingAPI.src.Modules.Booking.Application.Services.Interfaces;
using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Dto;
using BookingAPI.src.Modules.Booking.Domain.Dto.RoomDto;
using BookingAPI.src.Modules.Booking.Domain.Enums;
using BookingAPI.src.Modules.Booking.Domain.Mappers;
using BookingAPI.src.Modules.Booking.Infrastructure;
using BookingAPI.src.Shared.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace BookingAPI.src.Modules.Booking.Application.Services.Implementations;

public class RoomService(
    BookingDbContext context,
    HybridCache cache,
    ILogger<RoomService> logger
) : IRoomService
{
    private const string CACHE_TAG = "rooms";
    private const string LIST_CACHE_TAG = "rooms_list";

    public async Task<RoomResponseDto> Create(CreateRoomDto request)
    {
        logger.LogInformation("Criando Quarto - HotelId: {HotelId} - Code: {Code}", request.HotelId, request.Code);

        var hotel = await context
            .Hotels
            .FirstOrDefaultAsync(h => h.Id == request.HotelId)
            ?? throw new NullReferenceException("Hotel not found");

        var entity = new Room
        {
            Code = request.Code,
            Capacity = request.Capacity,
            HotelId = request.HotelId,
            Hotel = hotel,
            PricePerNight = request.PricePerNight,
            Reservations = []
        };

        context.Rooms.Add(entity);
        await context.SaveChangesAsync();

        var dto = RoomMapper.ToDto(entity);

        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        await cache.SetAsync(
            $"room:{dto.Id}", 
            dto, 
            tags: [CACHE_TAG]
        );

        logger.LogInformation("Quarto Criado - Id: {Id} - Code: {Code}", dto.Id, dto.Code);
        return dto;
    }

    public async Task<RoomResponseDto?> GetById(Guid id)
    {
        return await cache.GetOrCreateAsync(
            $"room:{id}",
            async ct => await context.Rooms
                .Select(r => RoomMapper.ToDto(r))
                .FirstOrDefaultAsync(r => r.Id == id, ct),
            tags: [CACHE_TAG]
        );
    }

    public async Task<PaginatedResult<RoomResponseDto>> List(ListRoomsDto request)
    {
        var queryable = context.Rooms
            .AsNoTracking()
            .WhereIf(request.HotelId is not null, e => e.HotelId == request.HotelId)
            .WhereIf(request.CodeALike is not null, e => e.Code.Contains(request.CodeALike!))
            .WhereIf(request.RoomTypes.Count > 0, e => request.RoomTypes.Contains(e.Type))
            .WhereIf(request.MinCapacity is not null, e => e.Capacity >= request.MinCapacity)
            .WhereIf(request.MaxCapacity is not null, e => e.Capacity <= request.MaxCapacity)
            .WhereIf(request.MinPrice is not null, e => e.PricePerNight >= request.MinPrice)
            .WhereIf(request.MaxPrice is not null, e => e.PricePerNight <= request.MaxPrice)
            .WhereIf(request.Price is not null, e => e.PricePerNight == request.Price);

        // Tratamento da lista de enums para compor a chave string sem quebrar
        var typesKey = request.RoomTypes.Count > 0 ? string.Join("_", request.RoomTypes) : "all";

        var cacheKey = $"rooms:list:p:{request.PageNumber}" +
                       $":h:{request.HotelId?.ToString() ?? "all"}" +
                       $":c:{request.CodeALike ?? "all"}" +
                       $":types:{typesKey}" +
                       $":minc:{request.MinCapacity ?? 0}" +
                       $":maxc:{request.MaxCapacity ?? 0}" +
                       $":minp:{request.MinPrice ?? 0}" +
                       $":maxp:{request.MaxPrice ?? 0}" +
                       $":pr:{request.Price ?? 0}";

        return await cache.GetOrCreateAsync(
            cacheKey,
            async ct => await queryable.ToPaginatedResultAsync(request.PageNumber, r => RoomMapper.ToDto(r), cancellationToken: ct),
            tags: [LIST_CACHE_TAG]
        );
    }

    public async Task<RoomResponseDto> Update(PatchRoomDto request)
    {
        logger.LogInformation("Atualizando Quarto - Id: {Id} - Code Request: {Code}", request.RoomId, request.Code);

        var room = await context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId)
            ?? throw new NullReferenceException($"Quarto {request.RoomId} não encontrado");
        
        if (request.Code is not null) room.Code = request.Code;
        if (request.Capacity.HasValue) room.Capacity = (int)request.Capacity;
        if (request.PricePerNight.HasValue) room.PricePerNight = (int)request.PricePerNight;
        if (request.Type.HasValue) room.Type = (RoomType)request.Type;
        if (request.IsAvailable.HasValue) room.IsAvailable = (bool)request.IsAvailable;

        await context.SaveChangesAsync();
        var dto = RoomMapper.ToDto(room);

        await cache.RemoveAsync($"room:{request.RoomId}");
        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        await cache.SetAsync($"room:{dto.Id}", dto, tags: [CACHE_TAG]);

        logger.LogInformation("Quarto Atualizado - Id: {Id} - Code: {Code}", dto.Id, dto.Code);
        return dto;
    }

    public async Task Restore(Guid id)
    {
        logger.LogInformation("Restaurando Quarto - Id: {Id}", id);

        var room = await context.Rooms
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new NullReferenceException($"Quarto de Id = {id} não encontrado");

        room.IsDeleted = false;
        await context.SaveChangesAsync();

        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        await cache.SetAsync($"room:{room.Id}", RoomMapper.ToDto(room), tags: [CACHE_TAG]);
    }

    public async Task SoftDelete(Guid id)
    {
        logger.LogInformation("Soft Delete Quarto - Id: {Id}", id);

        var room = await context.Rooms
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new NullReferenceException($"Quarto de Id = {id} não encontrado");

        room.IsDeleted = true;
        
        await cache.RemoveAsync($"room:{id}");
        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        
        await context.SaveChangesAsync();
    }

    public async Task HardDelete(Guid id)
    {
        logger.LogInformation("Hard Delete Quarto - Id: {Id}", id);

        var room = await context.Rooms
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new NullReferenceException($"Quarto de Id = {id} não encontrado");

        context.Rooms.Remove(room);
        
        await cache.RemoveAsync($"room:{id}");
        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        
        await context.SaveChangesAsync();
    }
}