using BookingAPI.src.Modules.Booking.Domain.Enums;

namespace BookingAPI.src.Modules.Booking.Domain.Dto.RoomDto;

public record CreateRoomDto(
    Guid HotelId,
    string Code,
    RoomType Type,
    int Capacity,
    decimal PricePerNight
);