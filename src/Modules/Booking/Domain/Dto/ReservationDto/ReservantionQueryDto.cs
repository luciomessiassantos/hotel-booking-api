using BookingAPI.src.Modules.Booking.Domain.Enums;

namespace BookingAPI.src.Modules.Booking.Domain.Dto.ReservationDto;

public record ReservationQueryDto(Guid? GuestId, Guid? RoomId, ReservationStatus? Status, int PageNumber = 1, bool? Deleted = false);