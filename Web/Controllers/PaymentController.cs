using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Infrastructure.Services;

namespace Web.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPayPalService _payPalService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPayPalService payPalService, ILogger<PaymentController> logger)
        {
            _payPalService = payPalService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string bookingId)
        {
            if (string.IsNullOrEmpty(bookingId))
            {
                TempData["Error"] = "Booking ID is required.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var existingPayment = await _payPalService.GetPaymentByBookingIdAsync(bookingId);
                if (existingPayment != null && existingPayment.IsPaid)
                {
                    TempData["Success"] = "This booking has already been paid for.";
                    return RedirectToAction("BookingDetails", "Booking", new { id = bookingId });
                }

                ViewBag.BookingId = bookingId;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking payment status for booking {BookingId}", bookingId);
                TempData["Error"] = "An error occurred while processing your request.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePayment(string bookingId, decimal amount)
        {
            if (string.IsNullOrEmpty(bookingId) || amount <= 0)
            {
                TempData["Error"] = "Invalid booking details.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var payment = await _payPalService.CreatePayment(amount, bookingId);

                if (!string.IsNullOrEmpty(payment.ApproveUrl))
                {
                    return Redirect(payment.ApproveUrl);
                }

                TempData["Error"] = "Failed to create payment. Please try again.";
                return RedirectToAction("Index", new { bookingId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for booking {BookingId}", bookingId);
                TempData["Error"] = "An error occurred while processing your payment. Please try again.";
                return RedirectToAction("Index", new { bookingId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Success(string token, string PayerID, string bookingId)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Payment token is missing.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Get the order status first
                var orderStatus = await _payPalService.GetOrderStatusAsync(token);

                if (orderStatus == "APPROVED")
                {
                    // Capture the payment
                    var captureResult = await _payPalService.CapturePaymentAsync(token);

                    TempData["Success"] = "Payment completed successfully! Your booking has been confirmed.";
                    _logger.LogInformation("Payment captured successfully for order {OrderId}", token);

                    // Redirect to booking details or confirmation page
                    if (!string.IsNullOrEmpty(bookingId))
                    {
                        return RedirectToAction("BookingDetails", "Booking", new { id = bookingId });
                    }

                    return RedirectToAction("PaymentConfirmation", new { orderId = token });
                }
                else
                {
                    TempData["Warning"] = $"Payment status: {orderStatus}. Please contact support if you think this is an error.";
                    return RedirectToAction("Index", new { bookingId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment success for token {Token}", token);
                TempData["Error"] = "An error occurred while confirming your payment. Please contact support.";
                return RedirectToAction("Index", new { bookingId });
            }
        }

        [HttpGet]
        public IActionResult Cancel(string bookingId)
        {
            TempData["Warning"] = "Payment was cancelled. You can try again when you're ready.";

            if (!string.IsNullOrEmpty(bookingId))
            {
                return RedirectToAction("Index", new { bookingId });
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> PaymentConfirmation(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                ViewBag.OrderId = orderId;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error displaying payment confirmation for order {OrderId}", orderId);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentStatus(string bookingId)
        {
            if (string.IsNullOrEmpty(bookingId))
            {
                return Json(new { success = false, message = "Booking ID is required." });
            }

            try
            {
                var payment = await _payPalService.GetPaymentByBookingIdAsync(bookingId);

                if (payment == null)
                {
                    return Json(new { success = false, message = "Payment not found." });
                }

                return Json(new
                {
                    success = true,
                    payment = new
                    {
                        id = payment.Id,
                        bookingId = payment.BookingId,
                        amount = payment.Amount,
                        currency = payment.Currency,
                        status = payment.Status,
                        isPaid = payment.IsPaid,
                        createdAt = payment.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment status for booking {BookingId}", bookingId);
                return Json(new { success = false, message = "An error occurred while checking payment status." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefreshPaymentStatus(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return Json(new { success = false, message = "Order ID is required." });
            }

            try
            {
                var status = await _payPalService.GetOrderStatusAsync(orderId);
                return Json(new { success = true, status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing payment status for order {OrderId}", orderId);
                return Json(new { success = false, message = "Failed to refresh payment status." });
            }
        }

        // Helper method to get current user ID
        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}