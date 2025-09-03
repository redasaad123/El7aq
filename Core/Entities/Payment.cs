using El7aq.Domain.Enums;

namespace El7aq.Domain.Entities
{
    public class Payment
    {
        public string Id { get; set; }
        public string BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PaymentStatus Status { get; set; }
        public PaymentMethod Method { get; set; }
        public string TransactionReference { get; set; }

        // Navigation
        public string PassengerId { get; set; }
        public PassengerProfile Passenger { get; set; }

    }


}
