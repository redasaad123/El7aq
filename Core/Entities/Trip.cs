using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Trip
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey(nameof(Route))]
        public required string RouteId { get; set; }

        [Required]
        [ForeignKey(nameof(Driver))]
        public required string DriverId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public int AvailableSeats { get; set; }

        // Navigation
        public Route? Route { get; set; }
        public DriverProfile? Driver { get; set; }
        public ICollection<Booking>? Bookings { get; set; } = new List<Booking>();
        public ICollection<TripDriverQueue>? DriverQueue { get; set; } = new List<TripDriverQueue>();
    }
}
