
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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

        // 🛡️ Identity Tables
        builder.Entity<AppUsers>().ToTable("Users", "security");
        builder.Entity<IdentityRole>().ToTable("Roles", "security");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRole", "security");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim", "security");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin", "security");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim", "security");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserToken", "security");

        // 🔒 Ensure email uniqueness
        builder.Entity<AppUsers>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // 🟢 Ensure one-to-one profiles per user
        builder.Entity<DriverProfile>()
            .HasIndex(d => d.UserId)
            .IsUnique();

        builder.Entity<PassengerProfile>()
            .HasIndex(p => p.UserId)
            .IsUnique();

        builder.Entity<StaffProfile>()
            .HasIndex(s => s.UserId)
            .IsUnique();

        builder.Entity<ManagerProfile>()
            .HasIndex(m => m.UserId)
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

        // ManagerProfile ↔ AppUsers (1:1)
        builder.Entity<ManagerProfile>()
            .HasOne(m => m.User)
            .WithOne()
            .HasForeignKey<ManagerProfile>(m => m.UserId)
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
            .HasOne(p => p.Booking)
            .WithMany(b => b.Payments)
            .HasForeignKey(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Payment ↔ Passenger
        builder.Entity<Payment>()
            .HasOne(p => p.Passenger)
            .WithMany(pp => pp.Payments)
            .HasForeignKey(p => p.PassengerId)
            .OnDelete(DeleteBehavior.Restrict);


      
        // 4. DriverProfile
        builder.Entity<DriverProfile>().HasData(
            new DriverProfile { Id = "D1", UserId = "1d9f8228-d327-4d93-9cfc-02835fd7bbf4", LicenseNumber = "LIC123", CarNumber = "CAR123" }
        );

        // 5. PassengerProfile
        builder.Entity<PassengerProfile>().HasData(
            new PassengerProfile { Id = "P1", UserId = "207a1b24-2482-4c8e-8972-bb587f5d8287" }
        );


        // 7. Booking
        builder.Entity<Booking>().HasData(
            new Booking { Id = "B1", PassengerId = "P1", TripId = "T1", BookingDate = DateTime.UtcNow, Status = BookingStatus.Pending },
            new Booking { Id = "B3", PassengerId = "P1", TripId = "T1", BookingDate = DateTime.UtcNow, Status = BookingStatus.Pending }

        );
        builder.Entity<Booking>().HasData(
            new Booking { Id = "B2", PassengerId = "P1", TripId = "T1", BookingDate = DateTime.UtcNow, Status = BookingStatus.Pending }

        );

        // 8. Notifications 
        builder.Entity<Notification>().HasData(
            new Notification
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc",
                Message = "Welcome to El7aq! Your account was created successfully.",
                IsRead = true,
                CreatedAt = DateTime.UtcNow
            },
            new Notification
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc",
                Message = "Your first booking is pending confirmation.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddMinutes(-15)
            },
            new Notification
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc",
                Message = "ay 7aga 1111.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddMinutes(+20)
            },
            new Notification
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc",
                Message = "ay 7aga 22222.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddMinutes(-5)
            }
        );
    }

    //  DbSets
    public DbSet<DriverProfile> Drivers { get; set; }
    public DbSet<PassengerProfile> Passengers { get; set; }
    public DbSet<StaffProfile> Staffs { get; set; }
    public DbSet<ManagerProfile> Managers { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Notification> Notifications { get; set; }
}
