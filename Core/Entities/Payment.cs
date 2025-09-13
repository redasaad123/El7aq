using Core.Entities;
using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Payment
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public required string BookingId { get; set; }  // FK → Booking

    [Required]
    public required string PassengerId { get; set; } // FK → Passenger

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public required string Currency { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public PaymentStatus Status { get; set; }
    public PaymentMethod Method { get; set; }

    [Required]
    public required string TransactionReference { get; set; }

    // Navigation
    public PassengerProfile? Passenger { get; set; }
    public Booking? Booking { get; set; }
}
}
