using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Enums;
using MediatR;

namespace BookingAPI.src.Modules.Booking.Domain.Dto.HotelDtos;

public record PatchHotelDto(
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
);