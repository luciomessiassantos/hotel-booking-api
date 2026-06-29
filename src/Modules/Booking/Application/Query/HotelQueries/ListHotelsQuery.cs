using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Shared.Utils;
using MediatR;

namespace BookingAPI.src.Modules.Booking.Application.Query.HotelQueries;

public record ListHotelQuery(
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
    int PageNumber = 1
    
) : IRequest<PaginatedResult<Hotel>>;