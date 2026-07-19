namespace BookingAPI.src.Modules.Booking.Domain.Dto.GuestDto;


public record UpdateGuestDto(Guid GuestId, string? FullName, string? Email, string? Phone);