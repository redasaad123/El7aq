using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities
{
    public class Payment
    {
        
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Booking))]
        public string BookingId { get; set; }  

        [Required]
        [ForeignKey(nameof(Passenger))]
        public string PassengerId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Enums.PaymentStatus Status { get; set; }
        public Enums.PaymentMethod Method { get; set; }

        [Required]
        public string TransactionReference { get; set; }

        // Navigation
        public PassengerProfile Passenger { get; set; }
        public Booking Booking { get; set; } 
    }
}
