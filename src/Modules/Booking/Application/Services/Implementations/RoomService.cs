using BookingAPI.src.Modules.Booking.Application.Services.Interfaces;
using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Dto;
using BookingAPI.src.Modules.Booking.Domain.Dto.RoomDto;
using BookingAPI.src.Modules.Booking.Infrastructure;
using BookingAPI.src.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace BookingAPI.src.Modules.Booking.Application.Services.Implementations;

public class RoomService(
    BookingDbContext context
) : IRoomService
{
    public async Task<Room> Create(CreateRoomDto request)
    {
        var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Id == request.HotelId)
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

        var result = await context.Rooms.AddAsync(entity);
        await context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<RoomResponseDto?> GetById(Guid id)
    {
        return await context.Rooms
        .Select(r => new RoomResponseDto(
            r.Id, 
            r.HotelId,
            r.Code,
            r.Type,
            r.Capacity,
            r.PricePerNight,
            r.IsAvailable
        ))
        .FirstOrDefaultAsync(r => r.Id == id);
    }

    public Task HardDelete(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<PaginatedResult<RoomResponseDto>> List(ListRoomsDto request)
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

    return queryable.ToPaginatedResultAsync(request.PageNumber, r =>
        new RoomResponseDto(
            r.Id,
            r.HotelId,
            r.Code,
            r.Type,
            r.Capacity,
            r.PricePerNight,
            r.IsAvailable
        ));

        
            
    }

    public Task<Room> Restore(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task SoftDelete(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Room> Update(PatchRoomDto request)
    {
        throw new NotImplementedException();
    }
}