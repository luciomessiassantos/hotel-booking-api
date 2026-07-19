using BookingAPI.src.Modules.Booking.Domain.Enums;

namespace BookingAPI.src.Modules.Booking.Domain.Dto.RoomDto;

public record RoomResponseDto(
    Guid Id,
    Guid HotelId,
    string Code,
    RoomType Type,
    int Capacity,
    decimal PricePerNight,
    bool IsAvailable
);