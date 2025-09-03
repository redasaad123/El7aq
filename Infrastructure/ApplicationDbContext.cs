 using Core.Entities;
using El7aq.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ApplicationDbContext : IdentityDbContext<AppUsers>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUsers>().ToTable("users", "security");
        builder.Entity<IdentityRole>().ToTable("Roles", "security");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRole", "security");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim", "security");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin", "security");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim", "security");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserToken", "security");

        // Ensure one-to-one profiles per user
        builder.Entity<DriverProfile>()
            .HasIndex(d => d.UserId)
            .IsUnique();
        builder.Entity<PassengerProfile>()
            .HasIndex(p => p.UserId)
            .IsUnique();
        builder.Entity<StaffProfile>()
            .HasIndex(s => s.UserId)
            .IsUnique();

        // DriverProfile ↔ AppUsers (1:1)
        builder.Entity<DriverProfile>()
            .HasOne(d => d.appUsers)
            .WithOne()
            .HasForeignKey<DriverProfile>(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // PassengerProfile ↔ AppUsers (1:1)
        builder.Entity<PassengerProfile>()
            .HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<PassengerProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // StaffProfile ↔ AppUsers (1:1)
        builder.Entity<StaffProfile>()
            .HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<StaffProfile>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // StaffProfile ↔ Station
        builder.Entity<StaffProfile>()
            .HasOne(s => s.Station)
            .WithMany(st => st.Staff)
            .HasForeignKey(s => s.StationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Route ↔ Station (StartStation, EndStation)
        builder.Entity<Route>()
            .HasOne(r => r.StartStation)
            .WithMany(s => s.RoutesFrom)
            .HasForeignKey(r => r.StartStationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Route>()
            .HasOne(r => r.EndStation)
            .WithMany(s => s.RoutesTo)
            .HasForeignKey(r => r.EndStationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Booking ↔ Trip
        builder.Entity<Booking>()
            .HasOne(b => b.Trip)
            .WithMany(t => t.Bookings)
            .HasForeignKey(b => b.TripId)
            .OnDelete(DeleteBehavior.Restrict);

        // Booking ↔ Passenger
        builder.Entity<Booking>()
            .HasOne(b => b.Passenger)
            .WithMany(p => p.Bookings)
            .HasForeignKey(b => b.PassengerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Trip ↔ Driver
        builder.Entity<Trip>()
            .HasOne(t => t.Driver)
            .WithMany(d => d.Trips)
            .HasForeignKey(t => t.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Trip ↔ Route
        builder.Entity<Trip>()
            .HasOne(t => t.Route)
            .WithMany(r => r.Trips)
            .HasForeignKey(t => t.RouteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Payment ↔ Booking
        builder.Entity<Payment>()
            .HasOne<Booking>()
            .WithMany(b => b.Payments)
            .HasForeignKey(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

    }
    public DbSet<DriverProfile> Drivers { get; set; }
    public DbSet<PassengerProfile> Passengers { get; set; }
    public DbSet<StaffProfile> Staffs { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<Payment> Payments { get; set; }

}



