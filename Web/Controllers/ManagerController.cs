using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Core.Interfaces;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly ILogger<ManagerController> _logger;
        private readonly IUnitOfWork<PassengerProfile> _passengerUow;
        private readonly IUnitOfWork<AppUsers> _userUow;
        private readonly IUnitOfWork<Booking> _bookingUow;
        private readonly IUnitOfWork<Payment> _paymentUow;

        public ManagerController(
            ILogger<ManagerController> logger,
            IUnitOfWork<PassengerProfile> passengerUow,
            IUnitOfWork<AppUsers> userUow,
            IUnitOfWork<Booking> bookingUow,
            IUnitOfWork<Payment> paymentUow)
        {
            _logger = logger;
            _passengerUow = passengerUow;
            _userUow = userUow;
            _bookingUow = bookingUow;
            _paymentUow = paymentUow;
        }

        public IActionResult ManagerHome()
        {
            // Get the current user's name for display
            var firstName = User.FindFirst("FirstName")?.Value ?? User.Identity?.Name ?? "Manager";
            var lastName = User.FindFirst("LastName")?.Value ?? "";
            
            ViewBag.ManagerName = $"{firstName} {lastName}".Trim();
            
            return View();
        }

        public IActionResult Profile()
        {
            // Get the current user's name for display
            var firstName = User.FindFirst("FirstName")?.Value ?? User.Identity?.Name ?? "Manager";
            var lastName = User.FindFirst("LastName")?.Value ?? "";
            
            ViewBag.ManagerName = $"{firstName} {lastName}".Trim();
            
            return View();
        }

        public async Task<IActionResult> PassengerReports()
        {
            try
            {
                // Get all passengers with their user information and statistics
                var passengers = await _passengerUow.Entity.GetAllAsyncAsQuery()
                    .Include(p => p.User)
                    .Include(p => p.Bookings)
                    .Include(p => p.Payments)
                    .ToListAsync();

                // Create passenger report data
                var passengerReports = passengers.Select(p => new PassengerReportViewModel
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    FirstName = p.User?.FirstName ?? "Unknown",
                    LastName = p.User?.LastName ?? "Unknown",
                    Email = p.User?.Email ?? "Unknown",
                    PhoneNumber = p.User?.PhoneNumber ?? "Not provided",
                    TotalBookings = p.Bookings.Count,
                    TotalPayments = p.Payments.Count,
                    TotalSpent = p.Payments.Sum(pay => pay.Amount),
                    LastBookingDate = p.Bookings.OrderByDescending(b => b.BookingDate).FirstOrDefault()?.BookingDate,
                    RegistrationDate = p.User?.EmailConfirmed == true ? DateTime.Now.AddDays(-Random.Shared.Next(1, 365)) : null, // Mock registration date
                    IsActive = p.Bookings.Any(b => b.BookingDate > DateTime.Now.AddDays(-30)) // Active if booked in last 30 days
                }).OrderByDescending(p => p.TotalSpent).ToList();

                ViewBag.TotalPassengers = passengerReports.Count;
                ViewBag.ActivePassengers = passengerReports.Count(p => p.IsActive);
                ViewBag.TotalRevenue = passengerReports.Sum(p => p.TotalSpent);

                return View(passengerReports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading passenger reports");
                TempData["ErrorMessage"] = "Error loading passenger data. Please try again.";
                return View(new List<PassengerReportViewModel>());
            }
        }

        public async Task<IActionResult> GetPassengerDetails(string passengerId)
        {
            try
            {
                var passenger = await _passengerUow.Entity.GetAllAsyncAsQuery()
                    .Include(p => p.User)
                    .Include(p => p.Bookings)
                        .ThenInclude(b => b.Trip)
                            .ThenInclude(t => t.Route)
                    .Include(p => p.Payments)
                    .FirstOrDefaultAsync(p => p.Id == passengerId);

                if (passenger == null)
                {
                    return Json(new { success = false, message = "Passenger not found" });
                }

                var passengerDetails = new
                {
                    success = true,
                    data = new
                    {
                        Id = passenger.Id,
                        Name = $"{passenger.User?.FirstName} {passenger.User?.LastName}",
                        Email = passenger.User?.Email,
                        PhoneNumber = passenger.User?.PhoneNumber,
                        TotalBookings = passenger.Bookings.Count,
                        TotalSpent = passenger.Payments.Sum(p => p.Amount),
                        RecentBookings = passenger.Bookings
                            .OrderByDescending(b => b.BookingDate)
                            .Take(5)
                            .Select(b => new
                            {
                                BookingDate = b.BookingDate.ToString("yyyy-MM-dd"),
                                Status = b.Status.ToString()
                            }),
                        PaymentHistory = passenger.Payments
                            .OrderByDescending(p => p.CreatedAt)
                            .Take(5)
                            .Select(p => new
                            {
                                Amount = p.Amount,
                                Status = p.Status.ToString(),
                                Method = p.Method.ToString(),
                                Date = p.CreatedAt.ToString("yyyy-MM-dd")
                            })
                    }
                };

                return Json(passengerDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading passenger details for ID: {PassengerId}", passengerId);
                return Json(new { success = false, message = "Error loading passenger details" });
            }
        }
    }
}
