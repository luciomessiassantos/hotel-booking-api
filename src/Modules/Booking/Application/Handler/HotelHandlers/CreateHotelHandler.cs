using BookingAPI.src.Modules.Booking.Application.Command.HotelCommands;
using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Infrastructure;
using MediatR;

namespace BookingAPI.src.Modules.Booking.Application.Handler.HotelHandlers;

public class CreateHotelHandler(
    BookingDbContext context
) : IRequestHandler<CreateHotelCommand, Hotel>
{
    public async Task<Hotel> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
    {
        var entity = new Hotel
        {
            Name = request.Name,
            Address = request.Address,
            Amenities = request.Amenities,
            Rooms = [],
            StarRating = 0
        };

        var result = await context.Hotels.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }
}