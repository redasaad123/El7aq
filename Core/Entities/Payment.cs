using El7aq.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace El7aq.Domain.Entities
{
    public class Payment
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [ForeignKey(nameof(Booking))]
        public string BookingId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(10)]
        public string Currency { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PaymentStatus Status { get; set; }
        public PaymentMethod Method { get; set; }
        public string TransactionReference { get; set; }

        // Navigation
        public Booking? Booking { get; set; }

    }


}
