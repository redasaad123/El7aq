using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IPayPalService
    {
        Task<PaymentDTO> CreatePayment(decimal amount, string bookingId);
        Task<string> CapturePaymentAsync(string orderId);
        Task<string> GetOrderStatusAsync(string orderId);
        Task<string> GetAccessTokenAsync();
        Task<PaymentDTO?> GetPaymentByBookingIdAsync(string bookingId);

    }

    public class PaymentDTO
    {
        public string Id { get; set; }
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
