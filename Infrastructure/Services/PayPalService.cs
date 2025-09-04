//using AutoMapper;
//using Core.Interfaces;
//using Infrastructure;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http.Headers;
//using System.Security.Claims;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace Core
//{
//    public class PayPalService : IPayPalService
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly IMapper _mapper;
//        private readonly UserManager<Infrastructure.Entities.AppUsers> _userManager;
//        private readonly IConfiguration _configuration;
//        private readonly IEmailSend _emailSend;
//        private readonly string? client_Id;
//        private readonly string? clientSecret;
//        private readonly HttpClient _httpClient;
//        private readonly IHttpContextAccessor contextAccessor;

//        public PayPalService(
//            ApplicationDbContext db,
//            IMapper mapper,
//            UserManager<AppUsers> userManager,
//            IConfiguration configuration,
//            IEmailSend emailSend,
//            HttpClient httpClient,
//            IHttpContextAccessor httpContextAccessor)
//        {
//            _db = db;
//            _mapper = mapper;
//            _userManager = userManager;
//            _configuration = configuration;
//            _emailSend = emailSend;
//            client_Id = _configuration.GetValue<string>("PayPal:ClientId");
//            clientSecret = _configuration.GetValue<string>("PayPal:ClientSecret");
//            _httpClient = httpClient;
//            contextAccessor = httpContextAccessor;
//        }

//        public async Task<string> CapturePaymentAsync(string orderId)
//        {
//            var orderStatus = await GetOrderStatusAsync(orderId);
//            if (orderStatus != "APPROVED")
//            {
//                throw new Exception($"لا يمكن تنفيذ الطلب. الحالة الحالية: {orderStatus}");
//            }

//            var accessToken = await GetAccessTokenAsync();
//            var request = new HttpRequestMessage(
//                HttpMethod.Post,
//                $"https://api-m.sandbox.paypal.com/v2/checkout/orders/{orderId}/capture"
//            );

//            // تعيين headers
//            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//            request.Content = new StringContent("", Encoding.UTF8, "application/json");

//            var response = await _httpClient.SendAsync(request);
//            var resultJson = await response.Content.ReadAsStringAsync();

//            if (!response.IsSuccessStatusCode)
//            {
//                throw new Exception($"فشل تنفيذ دفعة PayPal: {resultJson}");
//            }

//            var payment = await _db.Payments.FirstOrDefaultAsync(p => p.TransactionReference == orderId);

//            if (payment != null)
//            {
//                payment.Status = Infrastructure.Enums.PaymentStatus.Success;
//                await _db.SaveChangesAsync();

//                var user = await _userManager.FindByIdAsync(payment.PassengerId);
//                if (user != null)
//                {
//                    await _emailSend.SendEmail(
//                        user.Email,
//                        "تأكيد الدفع",
//                        $"مرحباً {user.UserName} \n \n تم دفع طلبكم رقم {orderId} بنجاح للحجز رقم {payment.BookingId}");
//                }
//            }
//            return resultJson;
//        }

//        public async Task<string> GetOrderStatusAsync(string orderId)
//        {
//            var accessToken = await GetAccessTokenAsync();
//            var request = new HttpRequestMessage(
//                HttpMethod.Get,
//                $"https://api-m.sandbox.paypal.com/v2/checkout/orders/{orderId}"
//            );

//            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

//            var response = await _httpClient.SendAsync(request);
//            var jsonResponse = await response.Content.ReadAsStringAsync();

//            if (response.IsSuccessStatusCode)
//            {
//                using var doc = JsonDocument.Parse(jsonResponse);
//                if (doc.RootElement.TryGetProperty("status", out var status))
//                {
//                    return status.GetString() ?? "UNKNOWN";
//                }
//            }
//            return "UNKNOWN";
//        }

//        public async Task<PaymentDTO> CreatePayment(decimal amount, string bookingId)
//        {
//            var accessToken = await GetAccessTokenAsync();

//            _httpClient.DefaultRequestHeaders.Clear();
//            _httpClient.DefaultRequestHeaders.Authorization =
//                new AuthenticationHeaderValue("Bearer", accessToken);
//            _httpClient.DefaultRequestHeaders.Accept.Add(
//                new MediaTypeWithQualityHeaderValue("application/json"));

//            var currentPassengerId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
//            if (string.IsNullOrEmpty(currentPassengerId))
//                throw new Exception("المستخدم غير مصرح له بالوصول");

//            var request = contextAccessor.HttpContext?.Request;
//            var baseUrl = $"{request?.Scheme}://{request?.Host}";

//            var body = new
//            {
//                intent = "CAPTURE",
//                purchase_units = new[]
//                {
//                    new
//                    {
//                        amount = new
//                        {
//                            currency_code = "USD",
//                            value = amount.ToString("F2")
//                        },
//                        reference_id = bookingId.ToString()
//                    }
//                },
//                application_context = new
//                {
//                    return_url = $"{baseUrl}/Payment/Success",  // MVC Route
//                    cancel_url = $"{baseUrl}/Payment/Cancel",   // MVC Route
//                    user_action = "PAY_NOW"
//                }
//            };

//            var json = JsonSerializer.Serialize(body);

//            var content = new StringContent(json, Encoding.UTF8, "application/json");

//            var response = await _httpClient.PostAsync("https://api-m.sandbox.paypal.com/v2/checkout/orders", content);

//            var resultJson = await response.Content.ReadAsStringAsync();

//            Console.WriteLine("PAYPAL RESPONSE: " + resultJson);

//            if (!response.IsSuccessStatusCode)
//            {
//                throw new Exception($"erorr PayPal: {response.StatusCode}, {resultJson}");
//            }

//            using var doc = JsonDocument.Parse(resultJson);

//            var orderId = doc.RootElement.GetProperty("id").GetString();
//            var approveUrl = doc.RootElement
//                .GetProperty("links")
//                .EnumerateArray()
//                .First(x => x.GetProperty("rel").GetString() == "approve")
//                .GetProperty("href")
//                .GetString();

//            var payment = new Infrastructure.Entities.Payment
//            {
//                BookingId = bookingId,
//                PassengerId= currentPassengerId,
//                Amount = amount,
//                Currency = "USD",
//                CreatedAt = DateTime.UtcNow,
//                Status = Infrastructure.Enums.PaymentStatus.Pending,
//                Method = Infrastructure.Enums.PaymentMethod.Card,
//                TransactionReference = orderId
//            };

//            _db.Payments.Add(payment);
//            await _db.SaveChangesAsync();

//            var paymentDto = new PaymentDTO
//            {
//                Id = payment.Id,
//                BookingId = payment.BookingId,
//                Amount = payment.Amount,
//                Currency = payment.Currency,
//                CreatedAt = payment.CreatedAt,
//                Status = payment.Status.ToString(),
//                PassengerId = payment.PassengerId,
//                IsPaid = payment.Status == PaymentStatus.Pending,
//                ApproveUrl = approveUrl
//            };

//            return paymentDto;
//        }

//        public async Task<string> GetAccessTokenAsync()
//        {
//            var convert = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{client_Id}:{clientSecret}"));
//            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", convert);

//            var request_body = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

//            var response = await _httpClient.PostAsync("https://api-m.sandbox.paypal.com/v1/oauth2/token", request_body);

//            response.EnsureSuccessStatusCode();
//            var json = await response.Content.ReadAsStringAsync();
//            using var Doc = JsonDocument.Parse(json);
//            return Doc.RootElement.GetProperty("access_token").GetString() ?? "null";
//        }

//        public Task<PaymentDTO?> GetPaymentByBookingIdAsync(string bookingId)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
