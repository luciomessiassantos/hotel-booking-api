using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Dto.GuestDto;
using BookingAPI.src.Shared.Utils;

namespace BookingAPI.src.Modules.Booking.Application.Services.Interfaces;

public interface IGuestService : ICrudService<Guest, CreateGuestDto, UpdateGuestDto, ListGuestDto, GuestResponseDto, Guid>
{
    
}