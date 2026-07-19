using System.Text.Json.Serialization;

namespace BookingAPI.src.Modules.Booking.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoomType { 
    Single, 
    Double, 
    Triple, 
    Suite 
}