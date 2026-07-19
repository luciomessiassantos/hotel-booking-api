using BookingAPI.src.Modules.Booking.Domain.Dto.RoomDto;
using BookingAPI.src.Modules.Booking.Domain.Enums;
using BookingAPI.src.Modules.Booking.Domain.ValueObjects;

namespace BookingAPI.src.Modules.Booking.Domain.Dto.HotelDtos;

public record HotelResponseDto(
    Guid Id,
    string Name,
    Address Address,
    int StarRating,
    List<Amenity> Amenities,
    List<RoomResponseDto> Rooms
);