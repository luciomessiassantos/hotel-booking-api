using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Dto.ReservationDto;
using BookingAPI.src.Shared.Utils;

namespace BookingAPI.src.Modules.Booking.Application.Services.Interfaces;


public interface IReservationService : ICrudService<Reservation, CreateReservationDto, UpdateReservationDto, ReservationQueryDto, ReservationResponseDto, Guid>
{
    
}