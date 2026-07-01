using System.Runtime.CompilerServices;
using BookingAPI.src.Modules.Booking.Application.Query.HotelQueries;
using BookingAPI.src.Modules.Booking.Application.Services.Interfaces;
using BookingAPI.src.Modules.Booking.Domain.Dto;
using BookingAPI.src.Modules.Booking.Domain.Dto.HotelDtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingAPI.src.Modules.Booking.Api;

[ApiController]
[Route("api/booking")]
public class BookingController(
    IMediator mediator,
    IHotelService hotelService
) : ControllerBase
{

    [HttpGet("/hotels/{HotelId:guid}")]
    public async Task<IActionResult> GetHotelById([FromRoute] Guid HotelId)
    {
        var result = await hotelService.GetById(HotelId);

        return Ok(result);
    }

    [HttpPost("/hotels")]
    public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDto dto)
    {
        var result = await hotelService.Create(dto);

        return CreatedAtAction(nameof(GetHotelById), new { HotelId = result.Id }, result );
    }

    [HttpGet("/hotels")]
    public async Task<IActionResult> ListHotels([FromQuery] HotelQueryDto query)
    {
        var result = await hotelService.List(query);

        return Ok(result);
    }

    [HttpDelete("/hotels/{HotelId:guid}")]
    public async Task<IActionResult> SoftDeleteHotel([FromRoute] Guid HotelId)
    {
        await hotelService.SoftDelete(HotelId);

        return NoContent();
    }

    [HttpPatch("/hotels")]
    public async Task<IActionResult> PatchHotel( [FromBody] PatchHotelDto patchHotelDto)
    {
        var result = await hotelService.Update(patchHotelDto);

        return Ok(result);
    }

    [HttpPatch("/hotels/{HotelId:guid}/restore")]
    public async Task<IActionResult> RestoreHotel([FromRoute] Guid HotelId)
    {
        var result = await hotelService.Restore(HotelId);

        return Ok(result);
    }

    [HttpDelete("/hotel/{HotelId:guid}/hard-delete")]
    public async Task<IActionResult> HardDeleteHotel([FromRoute] Guid HotelId)
    {
        await hotelService.HardDelete(HotelId);

        return NoContent();
    }




}