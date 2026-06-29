using System.ComponentModel;
using BookingAPI.src.Modules.Booking.Application.Command.HotelCommands;
using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Enums;
using BookingAPI.src.Modules.Booking.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingAPI.src.Modules.Booking.Application.Handler.HotelHandlers;


public class PatchHotelHandler(
    BookingDbContext context
) : IRequestHandler<PatchHotelCommand, Hotel>
{
    public async Task<Hotel> Handle(PatchHotelCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Hotels
            .FirstOrDefaultAsync(h => h.Id == request.HotelId, cancellationToken)
            ?? throw new NullReferenceException($"Hotel de Id = {request.HotelId} não encontrado");

        if (request.Name is not null)
        {
            entity.Name = request.Name;
        }

        if (request.StarRating is not null)
        {
            entity.StarRating = (int) request.StarRating;
        }

        if (request.City is not null)
        {
            entity.Address = entity.Address with { City = request.City };
        }

        if (request.Street is not null)
        {
            entity.Address = entity.Address with { Street = request.Street };
        }

        if (request.Country is not null)
        {
            entity.Address = entity.Address with { Country = request.Country };
        }

        if (request.State is not null)
        {
            entity.Address = entity.Address with { State = request.State };
        }

        if (request.ZipCode is not null)
        {
            entity.Address = entity.Address with { ZipCode = request.ZipCode };
        }

        if (request.Number is not null)
        {
            entity.Address = entity.Address with { Number = request.Number };
        }

        if (request.Amenities is not null)
        {
            entity.Amenities = request.Amenities; 
        }


        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}