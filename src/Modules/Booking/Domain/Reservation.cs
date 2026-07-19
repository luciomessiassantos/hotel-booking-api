using BookingAPI.src.Modules.Booking.Domain.Enums;
using BookingAPI.src.Shared.Utils;

namespace BookingAPI.src.Modules.Booking.Domain;

public class Reservation : AuditableEntity<Guid>
{

    public Guid GuestId { get; set; }
    public Guest Guest { get; set; } = null!;
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }    
    public ReservationStatus Status { get; set; }

}