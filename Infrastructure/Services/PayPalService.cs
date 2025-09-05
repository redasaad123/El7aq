using AutoMapper;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUsers> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSend _emailSend;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _currency;

        public PayPalService(
            ApplicationDbContext db,
            IMapper mapper,
            UserManager<AppUsers> userManager,
            IConfiguration configuration,
            IEmailSend emailSend,
            HttpClient httpClient,
            IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _emailSend = emailSend;
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;

            _clientId = _configuration.GetValue<string>("PayPal:ClientId") ?? throw new ArgumentNullException("PayPal ClientId missing");
            _clientSecret = _configuration.GetValue<string>("PayPal:ClientSecret") ?? throw new ArgumentNullException("PayPal ClientSecret missing");
            _currency = _configuration.GetValue<string>("PayPal:Currency") ?? "USD";
        }

        public async Task<string> CapturePaymentAsync(string orderId)
        {
            var orderStatus = await GetOrderStatusAsync(orderId);
            if (orderStatus != "APPROVED")
                throw new Exception($"Cannot capture order. Current status: {orderStatus}");

            var accessToken = await GetAccessTokenAsync();

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://api-m.sandbox.paypal.com/v2/checkout/orders/{orderId}/capture"
            );
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent("", Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var resultJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"PayPal capture error: {resultJson}");

            var payment = await _db.Payments.FirstOrDefaultAsync(p => p.TransactionReference == orderId);
            if (payment != null)
            {
                payment.Status = PaymentStatus.Success;
                await _db.SaveChangesAsync();

                var user = await _userManager.FindByIdAsync(payment.PassengerId);
                if (user != null)
                {
                    await _emailSend.SendEmail(
                        user.Email,
                        "Payment Confirmation",
                        $"Hello {user.UserName},\n\nYour order #{orderId} has been successfully paid for booking #{payment.BookingId}."
                    );
                }
            }

            return resultJson;
        }

        public async Task<string> GetOrderStatusAsync(string orderId)
        {
            var accessToken = await GetAccessTokenAsync();

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://api-m.sandbox.paypal.com/v2/checkout/orders/{orderId}"
            );
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(jsonResponse);
                if (doc.RootElement.TryGetProperty("status", out var status))
                    return status.GetString() ?? "UNKNOWN";
            }
            return "UNKNOWN";
        }

        public async Task<PaymentDTO> CreatePayment(decimal amount, string bookingId)
        {
            var accessToken = await GetAccessTokenAsync();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var currentUserId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
                throw new Exception("User not authenticated");

            var passenger = await _db.Passengers.FirstOrDefaultAsync(p => p.UserId == currentUserId);
            if (passenger == null)
                throw new Exception("Passenger profile not found for current user");

            var request = _contextAccessor.HttpContext?.Request;
            var baseUrl = $"{request?.Scheme}://{request?.Host}";

            var body = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency_code = _currency,
                            value = amount.ToString("F2")
                        },
                        reference_id = bookingId
                    }
                },
                application_context = new
                {
                    return_url = $"{baseUrl}/Payment/Success?bookingId={bookingId}", 
                    cancel_url = $"{baseUrl}/Payment/Cancel?bookingId={bookingId}",  
                    user_action = "PAY_NOW"
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api-m.sandbox.paypal.com/v2/checkout/orders", content);
            var resultJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"PayPal error: {response.StatusCode}, {resultJson}");

            using var doc = JsonDocument.Parse(resultJson);
            var orderId = doc.RootElement.GetProperty("id").GetString();
            var approveUrl = doc.RootElement
                .GetProperty("links")
                .EnumerateArray()
                .First(x => x.GetProperty("rel").GetString() == "approve")
                .GetProperty("href")
                .GetString();

            var payment = new Payment
            {
                BookingId = bookingId,
                PassengerId = passenger.Id,   
                Amount = amount,
                Currency = _currency,
                CreatedAt = DateTime.UtcNow,
                Status = PaymentStatus.Pending,
                Method = PaymentMethod.Card,
                TransactionReference = orderId
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            return new PaymentDTO
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                CreatedAt = payment.CreatedAt,
                Status = payment.Status.ToString(),
                PassengerId = payment.PassengerId,
                IsPaid = payment.Status == PaymentStatus.Success,
                ApproveUrl = approveUrl
            };
        }

        public async Task<string> GetAccessTokenAsync()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);

            var content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync("https://api-m.sandbox.paypal.com/v1/oauth2/token", content);

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("access_token").GetString() ?? throw new Exception("Failed to get access token");
        }

        public async Task<PaymentDTO?> GetPaymentByBookingIdAsync(string bookingId)
        {
            var payment = await _db.Payments.FirstOrDefaultAsync(p => p.BookingId == bookingId);
            if (payment == null) return null;

            return new PaymentDTO
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                CreatedAt = payment.CreatedAt,
                Status = payment.Status.ToString(),
                PassengerId = payment.PassengerId,
                IsPaid = payment.Status == PaymentStatus.Success
            };
        }
    }
}
