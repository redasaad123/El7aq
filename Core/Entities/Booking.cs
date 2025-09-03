using El7aq.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace El7aq.Domain.Entities
{
    public class Booking
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [ForeignKey(nameof(Passenger))]
        public string PassengerId { get; set; }

        [Required]
        [ForeignKey(nameof(Trip))]
        public string TripId { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus Status { get; set; }

        // Navigation
        public PassengerProfile? Passenger { get; set; }
        public Trip? Trip { get; set; }
        public ICollection<Payment>? Payments { get; set; } = new List<Payment>();
    }


}
