using System.Net.Sockets;
using BookingAPI.src.Modules.Booking.Domain.Enums;
using BookingAPI.src.Modules.Booking.Domain.ValueObjects;
using BookingAPI.src.Shared.Utils;

namespace BookingAPI.src.Modules.Booking.Domain;

public class Hotel : AuditableEntity
{
    public required string Name { get; set; }
    public required Address Address { get; set; }
    public int StarRating { get; set; }        
    public List<Amenity> Amenities { get; set; } = [];
    public List<Room> Rooms { get; set; } = [];
}