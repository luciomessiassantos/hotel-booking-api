namespace BookingAPI.src.Modules.Booking.Domain.Dto.HotelDtos;

public record HotelQueryDto(
    string? Term,
    string? Country,
    string? State,
    string? City,
    string? Street,
    int? MinRating,
    int? MaxRating,
    decimal? MaxRoomPrice,
    decimal? MinRoomPrice,
    bool? Deleted,
    bool? DeletedOnly,
    int PageNumber = 1
);