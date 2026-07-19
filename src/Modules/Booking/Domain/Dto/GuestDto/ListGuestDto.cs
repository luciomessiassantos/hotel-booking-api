namespace BookingAPI.src.Modules.Booking.Domain.Dto.GuestDto;

public record ListGuestDto(string? Term, int PageNumber = 1, bool? Deleted = false);