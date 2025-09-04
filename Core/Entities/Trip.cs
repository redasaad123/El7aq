using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Trip
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [ForeignKey(nameof(Route))]
        public string RouteId { get; set; }

        [Required]
        [ForeignKey(nameof(Driver))]
        public string DriverId { get; set; }

        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }

        

        // Navigation
        public Route? Route { get; set; }
        public DriverProfile? Driver { get; set; }
        public ICollection<Booking>? Bookings { get; set; } = new List<Booking>();
    }
}
