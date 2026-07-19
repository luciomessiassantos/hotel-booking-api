using BookingAPI.src.Modules.Booking.Domain.Dto;
using BookingAPI.src.Modules.Booking.Domain.Dto.RoomDto;
using BookingAPI.src.Shared.Utils;

namespace BookingAPI.src.Modules.Booking.Domain.Mappers;

public class RoomMapper : IMapper<Room, RoomResponseDto>
{
    public static RoomResponseDto ToDto(Room entity)
    {
        return new RoomResponseDto(
            entity.Id,
            entity.HotelId,
            entity.Code,
            entity.Type,
            entity.Capacity,
            entity.PricePerNight,
            entity.IsAvailable
        );
    }

    public static List<RoomResponseDto> ToDto(List<Room> entities) => entities.Select(e => ToDto(e)).ToList();
}