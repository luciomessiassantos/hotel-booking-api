using BookingAPI.src.Shared.Utils;

namespace BookingAPI.src.Modules.Booking.Domain;

public class Guest : AuditableEntity
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }

    public List<Reservation> Reservations { get; set; } = [];
}
