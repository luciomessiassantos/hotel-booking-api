using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Dto;
using BookingAPI.src.Modules.Booking.Domain.Dto.HotelDtos;
using BookingAPI.src.Shared.Utils;

namespace BookingAPI.src.Modules.Booking.Application.Services.Interfaces;

public interface IHotelService : ICrudService<Hotel, CreateHotelDto, PatchHotelDto, HotelQueryDto, HotelResponseDto, Guid>
{

}