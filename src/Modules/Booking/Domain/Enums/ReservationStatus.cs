using System.Text.Json.Serialization;

namespace BookingAPI.src.Modules.Booking.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReservationStatus 
{ 
    Pending, 
    Confirmed, 
    Cancelled, 
    Completed 
}
