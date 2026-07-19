namespace BookingAPI.src.Modules.Booking.Domain.Dto.GuestDto;

public record GuestResponseDto(Guid Id, string FullName, string Email, string Phone);