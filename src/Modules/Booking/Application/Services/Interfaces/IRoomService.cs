using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Dto;
using BookingAPI.src.Modules.Booking.Domain.Dto.RoomDto;
using BookingAPI.src.Shared.Utils;

namespace BookingAPI.src.Modules.Booking.Application.Services.Interfaces;


public interface IRoomService : ICrudService<Room, CreateRoomDto, PatchRoomDto, ListRoomsDto, RoomResponseDto, Guid>
{
    
}