using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Dto;
using BookingAPI.src.Modules.Booking.Domain.Enums;
using MediatR;

namespace BookingAPI.src.Modules.Booking.Application.Command.HotelCommands;

public record PatchHotelCommand(
    Guid HotelId,
    string? Name,
    int? StarRating,
    string? Street,
    string? Number,
    string? City,
    string? State,
    string? Country,
    string? ZipCode,
    List<Amenity>? Amenities

) : IRequest<Hotel>;