using BookingAPI.src.Modules.Booking.Domain.Enums;

namespace BookingAPI.src.Modules.Booking.Domain.Dto.ReservationDto;

public record UpdateReservationDto(Guid ReservationId, DateTime? CheckInDate, DateTime? CheckOutDate, ReservationStatus? Status);