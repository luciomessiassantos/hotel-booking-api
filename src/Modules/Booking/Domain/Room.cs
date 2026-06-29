using BookingAPI.src.Modules.Booking.Domain.Enums;
using BookingAPI.src.Shared.Utils;

namespace BookingAPI.src.Modules.Booking.Domain;

public class Room : AuditableEntity
{

    public Guid HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;
    public required string Code { get; set; }           // ex: "101", "SUITE-A"
    public RoomType Type { get; set; }         // Single, Double, Suite...
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; }

    public List<Reservation> Reservations { get; set; } = [];
}