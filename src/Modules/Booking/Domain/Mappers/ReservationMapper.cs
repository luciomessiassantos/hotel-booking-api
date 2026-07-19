using BookingAPI.src.Modules.Booking.Domain.Dto.ReservationDto;
using BookingAPI.src.Shared.Utils;

namespace BookingAPI.src.Modules.Booking.Domain.Mappers;


public class ReservationMapper : IMapper<Reservation, ReservationResponseDto>
{
    public static ReservationResponseDto ToDto(Reservation entity) =>
        new(
            entity.Id,
            entity.GuestId,
            entity.Guest?.FullName ?? "N/A",
            entity.RoomId,
            entity.Room?.Code ?? "N/A",
            entity.CheckInDate,
            entity.CheckOutDate,
            entity.TotalPrice,
            entity.Status
        );
}