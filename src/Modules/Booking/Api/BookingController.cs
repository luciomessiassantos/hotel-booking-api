using BookingAPI.src.Modules.Booking.Application.Command.HotelCommands;
using BookingAPI.src.Modules.Booking.Application.Handler.HotelHandlers;
using BookingAPI.src.Modules.Booking.Application.Query.HotelQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingAPI.src.Modules.Booking.Api;

[ApiController]
[Route("api/booking")]
public class BookingController(
    IMediator mediator
) : ControllerBase
{

    [HttpGet("/hotels/{HotelId:guid}")]
    public async Task<IActionResult> GetHotelById([FromRoute] Guid HotelId)
    {
        var result = await mediator.Send(new GetHotelByIdQuery(HotelId));

        return Ok(result);
    }

    [HttpPost("/hotels")]
    public async Task<IActionResult> CreateHotel([FromBody] CreateHotelCommand command)
    {
        var result = await mediator.Send(command);

        return CreatedAtAction(nameof(GetHotelById), new { HotelId = result.Id }, result );
    }

    [HttpGet("/hotels")]
    public async Task<IActionResult> ListHotels([FromQuery] ListHotelQuery query)
    {
        var result = await mediator.Send(query);

        return Ok(result);
    }

    

}