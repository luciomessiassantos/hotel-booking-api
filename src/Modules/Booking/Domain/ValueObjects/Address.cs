namespace BookingAPI.src.Modules.Booking.Domain.ValueObjects;

public record Address(
    string Street,
    string Number,
    string City,
    string State,
    string Country,
    string ZipCode
);