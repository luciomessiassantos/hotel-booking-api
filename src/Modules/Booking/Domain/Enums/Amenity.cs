using System.Text.Json.Serialization;

namespace BookingAPI.src.Modules.Booking.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Amenity
{
    Wifi,
    Gym,
    Breakfast,
    Pool,
    ParkingLot,
    AirConditioning
}