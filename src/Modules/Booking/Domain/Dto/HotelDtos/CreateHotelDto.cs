using BookingAPI.src.Modules.Booking.Domain.Enums;
using BookingAPI.src.Modules.Booking.Domain.ValueObjects;

namespace BookingAPI.src.Modules.Booking.Domain.Dto;

public record CreateHotelDto(
    string Name,
    Address Address,
    List<Amenity> Amenities
);