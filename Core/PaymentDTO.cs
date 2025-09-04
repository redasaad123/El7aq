namespace Core
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public string BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPaid { get; set; }
        public string Status { get; set; }
        public string PassengerId { get; set; }
        public string? ApproveUrl { get; set; }

    }
}