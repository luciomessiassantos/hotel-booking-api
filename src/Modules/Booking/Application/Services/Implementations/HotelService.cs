using BookingAPI.src.Modules.Booking.Application.Services.Interfaces;
using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Dto;
using BookingAPI.src.Modules.Booking.Domain.Dto.HotelDtos;
using BookingAPI.src.Modules.Booking.Infrastructure;
using BookingAPI.src.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace BookingAPI.src.Modules.Booking.Application.Services.Implementations;

public class HotelService(
    BookingDbContext context
) : IHotelService
{
    public async Task<Hotel> Create(CreateHotelDto request)
    {
        var entity = new Hotel
        {
            Name = request.Name,
            Address = request.Address,
            Amenities = request.Amenities,
            Rooms = [],
            StarRating = 0
        };

        var result = await context.Hotels.AddAsync(entity);
        await context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<HotelResponseDto?> GetById(Guid id)
    {
        return await context.Hotels
            .Select(h => new HotelResponseDto(
                h.Id,
                h.Name,
                h.Address,
                h.StarRating,
                h.Amenities,
                h.Rooms.Select(r => new RoomResponseDto(
                    r.Id,
                    r.HotelId,
                    r.Code,
                    r.Type,
                    r.Capacity,
                    r.PricePerNight,
                    r.IsAvailable
                )).ToList()
            ))
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<PaginatedResult<HotelResponseDto>> List(HotelQueryDto request)
    {
                
        var queryable = context.Hotels
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

        int totalCount = await queryable.CountAsync();

        return await queryable.ToPaginatedResultAsync(request.PageNumber, h => new HotelResponseDto(
                h.Id,
                h.Name,
                h.Address,
                h.StarRating,
                h.Amenities,
                h.Rooms.Select(r => new RoomResponseDto(
                    r.Id,
                    r.HotelId,
                    r.Code,
                    r.Type,
                    r.Capacity,
                    r.PricePerNight,
                    r.IsAvailable
                )).ToList()
            ));

    }

    public async Task<Hotel> Update(PatchHotelDto request)
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

        return entity;
    }

    public Task<Hotel> Restore(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task SoftDelete(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task HardDelete(Guid id)
    {
        throw new NotImplementedException();
    }

}