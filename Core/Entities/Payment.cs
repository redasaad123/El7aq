using El7aq.Domain.Enums;

namespace El7aq.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentMethod Method { get; set; }
        public string TransactionReference { get; set; }

        // Navigation
        public Booking Booking { get; set; }
    }


}
