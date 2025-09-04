//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Core.Interfaces
//{
//    public interface IPayPalService
//    {
//        Task<PaymentDTO> CreatePayment(decimal amount, string bookingId);
//        Task<string> CapturePaymentAsync(string orderId);
//        Task<string> GetOrderStatusAsync(string orderId);
//        Task<string> GetAccessTokenAsync();
//        Task<PaymentDTO?> GetPaymentByBookingIdAsync(string bookingId);

//    }
//}
