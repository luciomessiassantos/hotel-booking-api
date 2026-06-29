using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Dto;
using MediatR;

namespace BookingAPI.src.Modules.Booking.Application.Query.HotelQueries;

public record GetHotelByIdQuery(
    Guid HotelId
) : IRequest<HotelResponseDto?>;