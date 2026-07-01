using BookingAPI.src.Modules.Booking.Domain.Enums;

namespace BookingAPI.src.Modules.Booking.Domain.Dto.RoomDto;

public record ListRoomsDto(
    Guid? HotelId,
    string? CodeALike,
    List<RoomType> RoomTypes,
    int? MinCapacity,
    int? MaxCapacity,
    decimal? MinPrice,
    decimal? MaxPrice,
    decimal? Price,
    
    int PageNumber = 1
);