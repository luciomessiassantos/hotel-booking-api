using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace BookingAPI.src.Modules.Booking.Infrastructure;

public class BookingDbContext : ApplicationDbContext
{
    public BookingDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {   }

    // public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureAuditableEntity<Hotel, Guid>(modelBuilder, "hotels");
        ConfigureAuditableEntity<Room, Guid>(modelBuilder, "rooms");
        ConfigureAuditableEntity<Guest, Guid>(modelBuilder, "guests");
        ConfigureAuditableEntity<Reservation, Guid>(modelBuilder, "reservations");

        ConfigureHotel(modelBuilder);
        ConfigureRoom(modelBuilder);
        ConfigureGuest(modelBuilder);
        ConfigureReservation(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }


    // private static void ConfigureAuditableEntity<T>(ModelBuilder modelBuilder, string tableName)
    //     where T : AuditableEntity<Guid>
    // {
    //     modelBuilder.Entity<T>(entity =>
    //     {
    //         entity.ToTable(tableName);

    //         entity.HasKey(e => e.Id);

    //         entity.Property(e => e.CreatedAt)
    //             .IsRequired();

    //         entity.Property(e => e.UpdatedAt)
    //             .IsRequired();

    //         entity.Property(e => e.RowVersion)
    //             .IsRowVersion()
    //             .IsConcurrencyToken();

    //         entity.Property(e => e.IsDeleted)
    //             .IsRequired()
    //             .HasDefaultValue(false);

    //         entity.Property(e => e.DeletedAt);
    //         entity.Property(e => e.DeletedBy);


    //         entity.HasQueryFilter(e => !e.IsDeleted);
    //     });
    // }



    private static void ConfigureHotel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.Property(h => h.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(h => h.StarRating)
                .IsRequired();

            entity.OwnsOne(h => h.Address, address =>
            {
                address.Property(a => a.Street).HasColumnName("street").HasMaxLength(200).IsRequired();
                address.Property(a => a.Number).HasColumnName("number").HasMaxLength(10).IsRequired();
                address.Property(a => a.City).HasColumnName("city").HasMaxLength(100).IsRequired();
                address.Property(a => a.State).HasColumnName("state").HasMaxLength(100).IsRequired();
                address.Property(a => a.Country).HasColumnName("country").HasMaxLength(100).IsRequired();
                address.Property(a => a.ZipCode).HasColumnName("zip_code").HasMaxLength(20).IsRequired();
            });


            entity.Property(h => h.Amenities)
                .HasColumnType("jsonb");

            entity.HasMany(h => h.Rooms)
                .WithOne(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }



    private static void ConfigureRoom(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>(entity =>
        {
            entity.Property(r => r.Code)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(r => r.Type)
                .IsRequired();

            entity.Property(r => r.Capacity)
                .IsRequired();

            entity.Property(r => r.PricePerNight)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(r => r.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true);

            entity.HasIndex(r => new { r.HotelId, r.Code })
                .IsUnique();
        });
    }



    private static void ConfigureGuest(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Guest>(entity =>
        {
            entity.Property(g => g.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(g => g.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(g => g.Phone)
                .HasMaxLength(20);

            entity.HasIndex(g => g.Email)
                .IsUnique();

            entity.HasMany(g => g.Reservations)
                .WithOne(r => r.Guest)
                .HasForeignKey(r => r.GuestId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }



    private static void ConfigureReservation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.Property(r => r.CheckInDate).IsRequired();
            entity.Property(r => r.CheckOutDate).IsRequired();

            entity.Property(r => r.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(r => r.Status)
                .IsRequired();

            entity.HasOne(r => r.Room)
                .WithMany(r => r.Reservations)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }



    // public override int SaveChanges()
    // {
    //     OnSaving();
    //     return base.SaveChanges();
    // }

    // public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    // {
    //     OnSaving();
    //     return await base.SaveChangesAsync(cancellationToken);
    // }

    // private void OnSaving()
    // {
    //     var entries = ChangeTracker.Entries<AuditableEntity<Guid>>();
    //     foreach (var entry in entries)
    //     {
    //         if (entry.State == EntityState.Added)
    //         {
    //             entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
    //             entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
    //         }

    //         if (entry.State == EntityState.Modified)
    //         {
    //             entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
    //         }

    //     }
    // }
}