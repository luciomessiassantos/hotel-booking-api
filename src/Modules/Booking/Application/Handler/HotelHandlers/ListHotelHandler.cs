using System.Runtime.CompilerServices;
using BookingAPI.src.Modules.Booking.Application.Query.HotelQueries;
using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Infrastructure;
using BookingAPI.src.Shared.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BookingAPI.src.Modules.Booking.Application.Handler.HotelHandlers;

public class ListHotelHandler(
    BookingDbContext context
) : IRequestHandler<ListHotelQuery, PaginatedResult<Hotel>>
{
    public async Task<PaginatedResult<Hotel>> Handle(ListHotelQuery request, CancellationToken cancellationToken)
    {
        var queryable = context.Hotels.AsQueryable();

        if (request.Term is not null)
        {
            queryable = queryable.Where(e => e.Name.Contains(request.Term));
        }

        if (request.Street is not null)
        {
            queryable = queryable.Where(e => e.Address.Street.Contains(request.Street));
        }

        if (request.City is not null)
        {
            queryable = queryable.Where(e => e.Address.City.Contains(request.City));
        }

        if (request.Country is not null)
        {
            queryable = queryable.Where(e => e.Address.Country.Contains(request.Country));
        }

        if (request.State is not null)
        {
            queryable = queryable.Where(e => e.Address.State.Contains(request.State));
        }

        if (request.MaxRating is not null)
        {
            queryable = queryable.Where(e => e.StarRating <= request.MaxRating);
        }

        if (request.MinRating is not null)
        {
            queryable = queryable.Where(e => e.StarRating >= request.MaxRating);
        }

        if (request.MaxRoomPrice is not null)
        {
            queryable = queryable.Where(e => e.Rooms.Any(r => r.PricePerNight <= request.MaxRoomPrice));
        }

        if (request.MinRoomPrice is not null)
        {
            queryable = queryable.Where(e => e.Rooms.Any(r => r.PricePerNight >= request.MinRoomPrice));
        }

        if (request.Deleted is not null and true)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        int pageSize = 12;
        int totalCount = await queryable.CountAsync(cancellationToken);
        var data = await queryable
            .Skip((request.PageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);


        return new PaginatedResult<Hotel>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = pageSize
        };


    }
}