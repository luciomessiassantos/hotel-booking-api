using BookingAPI.src.Modules.Booking.Application.Query.HotelQueries;
using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Dto;
using BookingAPI.src.Modules.Booking.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingAPI.src.Modules.Booking.Application.Handler.HotelHandlers;

public class GetHotelByIdHandler(
    BookingDbContext context
) : IRequestHandler<GetHotelByIdQuery, HotelResponseDto?>
{
    public async Task<HotelResponseDto?> Handle(GetHotelByIdQuery request, CancellationToken cancellationToken)
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
                    r.Code,
                    r.Type,
                    r.Capacity,
                    r.PricePerNight,
                    r.IsAvailable
                )).ToList()
            ))
            .FirstOrDefaultAsync(h => h.Id == request.HotelId, cancellationToken);
    }
}