namespace BookingAPI.src.Modules.Booking.Domain.Dto.ReservationDto;

public record CreateReservationDto(Guid GuestId, Guid RoomId, DateTime CheckInDate, DateTime CheckOutDate);