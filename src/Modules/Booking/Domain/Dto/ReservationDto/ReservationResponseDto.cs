using BookingAPI.src.Modules.Booking.Domain.Enums;

namespace BookingAPI.src.Modules.Booking.Domain.Dto.ReservationDto;


public record ReservationResponseDto(
    Guid Id, 
    Guid GuestId, 
    string GuestName, 
    Guid RoomId, 
    string RoomCode, 
    DateTime CheckInDate, 
    DateTime CheckOutDate, 
    decimal TotalPrice, 
    ReservationStatus Status);