using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Enums;
using BookingAPI.src.Modules.Booking.Domain.ValueObjects;
using MediatR;

namespace BookingAPI.src.Modules.Booking.Application.Command.HotelCommands;

public record CreateHotelCommand(
    string Name,
    Address Address,
    List<Amenity> Amenities
) : IRequest<Hotel>;