using BookingAPI.src.Modules.Booking.Application.Services.Interfaces;
using BookingAPI.src.Modules.Booking.Domain.Dto;
using BookingAPI.src.Modules.Booking.Domain.Dto.GuestDto;
using BookingAPI.src.Modules.Booking.Domain.Dto.HotelDtos;
using BookingAPI.src.Modules.Booking.Domain.Dto.ReservationDto;
using BookingAPI.src.Modules.Booking.Domain.Dto.RoomDto;
using Microsoft.AspNetCore.Mvc;

namespace BookingAPI.src.Modules.Booking.Api;

[ApiController]
[Route("api/booking")]
public class BookingController(
    IHotelService hotelService,
    IRoomService roomService,
    IGuestService guestService,
    IReservationService reservationService
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
        return CreatedAtAction(nameof(GetHotelById), new { HotelId = result.Id }, result);
    }

    [HttpGet("/hotels")]
    public async Task<IActionResult> ListHotels([FromQuery] HotelQueryDto query)
    {
        var result = await hotelService.List(query);
        return Ok(result);
    }

    [HttpPatch("/hotels")]
    public async Task<IActionResult> PatchHotel([FromBody] PatchHotelDto patchHotelDto)
    {
        var result = await hotelService.Update(patchHotelDto);
        return Ok(result);
    }

    [HttpDelete("/hotels/{HotelId:guid}")]
    public async Task<IActionResult> SoftDeleteHotel([FromRoute] Guid HotelId)
    {
        await hotelService.SoftDelete(HotelId);
        return NoContent();
    }

    [HttpPatch("/hotels/{HotelId:guid}/restore")]
    public async Task<IActionResult> RestoreHotel([FromRoute] Guid HotelId)
    {
        await hotelService.Restore(HotelId);
        return NoContent();
    }

    [HttpDelete("/hotels/{HotelId:guid}/hard-delete")]
    public async Task<IActionResult> HardDeleteHotel([FromRoute] Guid HotelId)
    {
        await hotelService.HardDelete(HotelId);
        return NoContent();
    }

    [HttpGet("/rooms/{RoomId:guid}")]
    public async Task<IActionResult> GetRoomById([FromRoute] Guid RoomId)
    {
        var result = await roomService.GetById(RoomId);
        return Ok(result);
    }

    [HttpPost("/rooms")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto dto)
    {
        var result = await roomService.Create(dto);
        return CreatedAtAction(nameof(GetRoomById), new { RoomId = result.Id }, result);
    }

    [HttpGet("/rooms")]
    public async Task<IActionResult> ListRooms([FromQuery] ListRoomsDto query)
    {
        var result = await roomService.List(query);
        return Ok(result);
    }

    [HttpPatch("/rooms")]
    public async Task<IActionResult> PatchRoom([FromBody] PatchRoomDto patchRoomDto)
    {
        var result = await roomService.Update(patchRoomDto);
        return Ok(result);
    }

    [HttpDelete("/rooms/{RoomId:guid}")]
    public async Task<IActionResult> SoftDeleteRoom([FromRoute] Guid RoomId)
    {
        await roomService.SoftDelete(RoomId);
        return NoContent();
    }

    [HttpPatch("/rooms/{RoomId:guid}/restore")]
    public async Task<IActionResult> RestoreRoom([FromRoute] Guid RoomId)
    {
        await roomService.Restore(RoomId);
        return NoContent();
    }

    [HttpDelete("/rooms/{RoomId:guid}/hard-delete")]
    public async Task<IActionResult> HardDeleteRoom([FromRoute] Guid RoomId)
    {
        await roomService.HardDelete(RoomId);
        return NoContent();
    }

    [HttpGet("/guests/{GuestId:guid}")]
    public async Task<IActionResult> GetGuestById([FromRoute] Guid GuestId)
    {
        var result = await guestService.GetById(GuestId);
        return Ok(result);
    }

    [HttpPost("/guests")]
    public async Task<IActionResult> CreateGuest([FromBody] CreateGuestDto dto)
    {
        var result = await guestService.Create(dto);
        return CreatedAtAction(nameof(GetGuestById), new { GuestId = result.Id }, result);
    }

    [HttpGet("/guests")]
    public async Task<IActionResult> ListGuests([FromQuery] ListGuestDto query)
    {
        var result = await guestService.List(query);
        return Ok(result);
    }

    [HttpPatch("/guests")]
    public async Task<IActionResult> PatchGuest([FromBody] UpdateGuestDto dto)
    {
        var result = await guestService.Update(dto);
        return Ok(result);
    }

    [HttpDelete("/guests/{GuestId:guid}")]
    public async Task<IActionResult> SoftDeleteGuest([FromRoute] Guid GuestId)
    {
        await guestService.SoftDelete(GuestId);
        return NoContent();
    }

    [HttpPatch("/guests/{GuestId:guid}/restore")]
    public async Task<IActionResult> RestoreGuest([FromRoute] Guid GuestId)
    {
        await guestService.Restore(GuestId);
        return NoContent();
    }

    [HttpDelete("/guests/{GuestId:guid}/hard-delete")]
    public async Task<IActionResult> HardDeleteGuest([FromRoute] Guid GuestId)
    {
        await guestService.HardDelete(GuestId);
        return NoContent();
    }




    [HttpGet("/reservations/{ReservationId:guid}")]
    public async Task<IActionResult> GetReservationById([FromRoute] Guid ReservationId)
    {
        var result = await reservationService.GetById(ReservationId);
        return Ok(result);
    }

    [HttpPost("/reservations")]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
    {
        var result = await reservationService.Create(dto);
        return CreatedAtAction(nameof(GetReservationById), new { ReservationId = result.Id }, result);
    }

    [HttpGet("/reservations")]
    public async Task<IActionResult> ListReservations([FromQuery] ReservationQueryDto query)
    {
        var result = await reservationService.List(query);
        return Ok(result);
    }

    [HttpPatch("/reservations")]
    public async Task<IActionResult> PatchReservation([FromBody] UpdateReservationDto dto)
    {
        var result = await reservationService.Update(dto);
        return Ok(result);
    }

    [HttpDelete("/reservations/{ReservationId:guid}")]
    public async Task<IActionResult> SoftDeleteReservation([FromRoute] Guid ReservationId)
    {
        await reservationService.SoftDelete(ReservationId);
        return NoContent();
    }

    [HttpPatch("/reservations/{ReservationId:guid}/restore")]
    public async Task<IActionResult> RestoreReservation([FromRoute] Guid ReservationId)
    {
        await reservationService.Restore(ReservationId);
        return NoContent();
    }

    [HttpDelete("/reservations/{ReservationId:guid}/hard-delete")]
    public async Task<IActionResult> HardDeleteReservation([FromRoute] Guid ReservationId)
    {
        await reservationService.HardDelete(ReservationId);
        return NoContent();
    }

}