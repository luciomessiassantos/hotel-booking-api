using BookingAPI.src.Modules.Booking.Domain.Dto.HotelDtos;
using BookingAPI.src.Shared.Utils;

namespace BookingAPI.src.Modules.Booking.Domain.Mappers;

public class HotelMapper : IMapper<Hotel, HotelResponseDto>
{
    public static HotelResponseDto ToDto(Hotel entity)
    {
        return new HotelResponseDto(
            entity.Id,
            entity.Name,
            entity.Address,
            entity.StarRating,
            entity.Amenities,
            RoomMapper.ToDto(entity.Rooms)
        );
    }

    public static List<HotelResponseDto> ToDto(List<Hotel> entities)
    {
        throw new NotImplementedException();
    }
}