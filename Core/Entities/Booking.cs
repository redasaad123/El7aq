using Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Booking
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey(nameof(Passenger))]
        public required string PassengerId { get; set; }

        [Required]
        [ForeignKey(nameof(Trip))]
        public required string TripId { get; set; }

        public DateTime BookingDate { get; set; }
        public BookingStatus Status { get; set; }

        // Navigation
        public PassengerProfile? Passenger { get; set; }
        public Trip? Trip { get; set; }
        public ICollection<Payment>? Payments { get; set; } = new List<Payment>();
    }
}
