using BookingAPI.src.Modules.Booking.Domain.Dto.GuestDto;
using BookingAPI.src.Shared.Utils;

namespace BookingAPI.src.Modules.Booking.Domain.Mappers;

public class GuestMapper : IMapper<Guest, GuestResponseDto>
{
    public static GuestResponseDto ToDto(Guest entity) =>
        new(entity.Id, entity.FullName, entity.Email, entity.Phone);
}