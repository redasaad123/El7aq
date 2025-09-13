using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Core.Interfaces;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Microsoft.AspNetCore.Identity;

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
        private readonly IUnitOfWork<StaffProfile> _staffUow;
        private readonly IUnitOfWork<Station> _stationUow;
        private readonly IUnitOfWork<DriverProfile> _driverUow;
        private readonly UserManager<AppUsers> _userManager;

        public ManagerController(
            ILogger<ManagerController> logger,
            IUnitOfWork<PassengerProfile> passengerUow,
            IUnitOfWork<AppUsers> userUow,
            IUnitOfWork<Booking> bookingUow,
            IUnitOfWork<Payment> paymentUow,
            IUnitOfWork<StaffProfile> staffUow,
            IUnitOfWork<Station> stationUow,
            IUnitOfWork<DriverProfile> driverUow,
            UserManager<AppUsers> userManager)
        {
            _logger = logger;
            _passengerUow = passengerUow;
            _userUow = userUow;
            _bookingUow = bookingUow;
            _paymentUow = paymentUow;
            _staffUow = staffUow;
            _stationUow = stationUow;
            _driverUow = driverUow;
            _userManager = userManager;
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
                var passengers = await _passengerUow.Entity.GetAllAsyncAsQuery()
                    .Include(p => p.User)
                    .Include(p => p.Bookings)
                    .Include(p => p.Payments)
                    .ToListAsync();

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
                    RegistrationDate = p.User?.EmailConfirmed == true ? DateTime.Now.AddDays(-Random.Shared.Next(1, 365)) : null,
                    IsActive = p.Bookings.Any(b => b.BookingDate > DateTime.Now.AddDays(-30))
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

        [HttpGet]
        public async Task<IActionResult> LoadPassengerReports(int page = 1, int pageSize = 12, string? search = null, string? status = null, string sortBy = "spent")
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 12;

                var baseQuery = _passengerUow.Entity.GetAllAsyncAsQuery();

                // Build filters on entity-level query
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var lowered = search.ToLower();
                    baseQuery = baseQuery.Where(p =>
                        ((p.User != null ? (p.User.FirstName + " " + p.User.LastName) : "").ToLower().Contains(lowered))
                        || (p.User != null && p.User.Email.ToLower().Contains(lowered)));
                }

                if (!string.IsNullOrWhiteSpace(status))
                {
                    var cutoff = DateTime.Now.AddDays(-30);
                    if (status.Equals("active", StringComparison.OrdinalIgnoreCase))
                        baseQuery = baseQuery.Where(p => p.Bookings.Any(b => b.BookingDate > cutoff));
                    else if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase))
                        baseQuery = baseQuery.Where(p => !p.Bookings.Any(b => b.BookingDate > cutoff));
                }

                // Stats computed with SQL-safe projections
                var cutoffForStats = DateTime.Now.AddDays(-30);
                var totalCount = await baseQuery.CountAsync();
                var activePassengers = await baseQuery.CountAsync(p => p.Bookings.Any(b => b.BookingDate > cutoffForStats));
                var totalRevenue = await baseQuery
                    .Select(p => p.Payments.Sum(pay => (decimal?)pay.Amount) ?? 0m)
                    .SumAsync();

                // Sorting using SQL projections
                IQueryable<Core.Entities.PassengerProfile> sortedQuery = sortBy switch
                {
                    "bookings" => baseQuery.OrderByDescending(p => p.Bookings.Count),
                    "name" => baseQuery.OrderBy(p => p.User != null ? p.User.FirstName : "").ThenBy(p => p.User != null ? p.User.LastName : ""),
                    "lastBooking" => baseQuery.OrderByDescending(p => p.Bookings.Max(b => (DateTime?)b.BookingDate)),
                    _ => baseQuery.OrderByDescending(p => p.Payments.Sum(pay => (decimal?)pay.Amount) ?? 0m)
                };

                // Page and project to view model in SQL
                var pagedItems = await sortedQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PassengerReportViewModel
                    {
                        Id = p.Id,
                        UserId = p.UserId,
                        FirstName = p.User != null ? p.User.FirstName : "Unknown",
                        LastName = p.User != null ? p.User.LastName : "Unknown",
                        Email = p.User != null ? p.User.Email : "Unknown",
                        PhoneNumber = p.User != null ? p.User.PhoneNumber : "Not provided",
                        TotalBookings = p.Bookings.Count,
                        TotalPayments = p.Payments.Count,
                        TotalSpent = p.Payments.Sum(pay => (decimal?)pay.Amount) ?? 0m,
                        LastBookingDate = p.Bookings
                            .OrderByDescending(b => b.BookingDate)
                            .Select(b => (DateTime?)b.BookingDate)
                            .FirstOrDefault(),
                        RegistrationDate = p.User != null && p.User.EmailConfirmed ? (DateTime?)DateTime.Now.AddDays(-30) : null,
                        IsActive = p.Bookings.Any(b => b.BookingDate > cutoffForStats)
                    })
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    items = pagedItems,
                    totalCount,
                    hasMore = page * pageSize < totalCount,
                    stats = new
                    {
                        totalPassengers = totalCount,
                        activePassengers,
                        totalRevenue
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading passenger reports page {Page}", page);
                return Json(new { success = false, message = "Error loading passengers" });
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

        public async Task<IActionResult> StaffReports()
        {
            try
            {
                var staffProfiles = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Include(s => s.User)
                    .Include(s => s.Station)
                    .ToListAsync();

                var staffReports = staffProfiles.Select(s => new StaffReportViewModel
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    StationId = s.StationId,
                    FirstName = s.User?.FirstName ?? "Unknown",
                    LastName = s.User?.LastName ?? "Unknown",
                    Email = s.User?.Email ?? "Unknown",
                    PhoneNumber = s.User?.PhoneNumber ?? "Not provided",
                    StationName = s.Station?.Name ?? "Unknown",
                    StationCity = s.Station?.City ?? "Unknown"
                }).OrderBy(s => s.StationName).ThenBy(s => s.FullName).ToList();

                ViewBag.TotalStaff = staffReports.Count;
                ViewBag.StationsCount = staffReports.Select(s => s.StationName).Distinct().Count();

                return View(staffReports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading staff reports");
                TempData["ErrorMessage"] = "Error loading staff data. Please try again.";
                return View(new List<StaffReportViewModel>());
            }
        }

        public async Task<IActionResult> DriverReports()
        {
            try
            {
                var drivers = await _driverUow.Entity.GetAllAsyncAsQuery()
                    .Include(d => d.appUsers)
                    .Include(d => d.Trips)
                    .ToListAsync();

                var reports = drivers.Select(d => new DriverReportViewModel
                {
                    Id = d.Id,
                    UserId = d.UserId,
                    FirstName = d.appUsers?.FirstName ?? "Unknown",
                    LastName = d.appUsers?.LastName ?? "Unknown",
                    Email = d.appUsers?.Email ?? "Unknown",
                    PhoneNumber = d.appUsers?.PhoneNumber ?? "Not provided",
                    LicenseNumber = d.LicenseNumber,
                    CarNumber = d.CarNumber,
                    TotalTrips = d.Trips.Count
                }).OrderByDescending(x => x.TotalTrips).ThenBy(x => x.FullName).ToList();

                ViewBag.TotalDrivers = reports.Count;
                ViewBag.ActiveDrivers = reports.Count(r => r.TotalTrips > 0);

                return View(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading driver reports");
                TempData["ErrorMessage"] = "Error loading driver data. Please try again.";
                return View(new List<DriverReportViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditPassenger(string id)
        {
            var passenger = await _passengerUow.Entity.GetAllAsyncAsQuery()
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (passenger == null) return NotFound();

            var vm = new PassengerEditViewModel
            {
                Id = passenger.Id,
                UserId = passenger.UserId,
                FirstName = passenger.User?.FirstName ?? string.Empty,
                LastName = passenger.User?.LastName ?? string.Empty,
                Email = passenger.User?.Email ?? string.Empty,
                PhoneNumber = passenger.User?.PhoneNumber
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPassenger(PassengerEditViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var passenger = await _passengerUow.Entity.GetAllAsyncAsQuery()
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == vm.Id);
            if (passenger == null) return NotFound();

            if (passenger.User != null)
            {
                passenger.User.FirstName = vm.FirstName;
                passenger.User.LastName = vm.LastName;
                passenger.User.Email = vm.Email;
                passenger.User.UserName = vm.Email;
                passenger.User.PhoneNumber = vm.PhoneNumber;
            }
            await _passengerUow.SaveAsync();
            TempData["Success"] = "Passenger updated successfully.";
            return RedirectToAction(nameof(PassengerReports));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePassenger(string id)
        {
            var passenger = await _passengerUow.Entity.GetAllAsyncAsQuery()
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (passenger == null)
            {
                TempData["ErrorMessage"] = "Passenger not found.";
                return RedirectToAction(nameof(PassengerReports));
            }

            _passengerUow.Entity.Delete(id);
            if (passenger.User != null)
            {
                _userUow.Entity.Delete(passenger.User.Id);
            }
            await _passengerUow.SaveAsync();

            TempData["Success"] = "Passenger deleted successfully.";
            return RedirectToAction(nameof(PassengerReports));
        }

        [HttpGet]
        public async Task<IActionResult> EditDriver(string id)
        {
            var driver = await _driverUow.Entity.GetAllAsyncAsQuery()
                .Include(d => d.appUsers)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (driver == null) return NotFound();

            var vm = new DriverReportViewModel
            {
                Id = driver.Id,
                UserId = driver.UserId,
                FirstName = driver.appUsers?.FirstName ?? string.Empty,
                LastName = driver.appUsers?.LastName ?? string.Empty,
                Email = driver.appUsers?.Email ?? string.Empty,
                PhoneNumber = driver.appUsers?.PhoneNumber,
                LicenseNumber = driver.LicenseNumber,
                CarNumber = driver.CarNumber,
                TotalTrips = 0
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDriver(DriverReportViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var driver = await _driverUow.Entity.GetAllAsyncAsQuery()
                .Include(d => d.appUsers)
                .FirstOrDefaultAsync(d => d.Id == vm.Id);
            if (driver == null) return NotFound();

            if (driver.appUsers != null)
            {
                driver.appUsers.FirstName = vm.FirstName;
                driver.appUsers.LastName = vm.LastName;
                driver.appUsers.Email = vm.Email;
                driver.appUsers.UserName = vm.Email;
                driver.appUsers.PhoneNumber = vm.PhoneNumber;
            }
            driver.LicenseNumber = vm.LicenseNumber;
            driver.CarNumber = vm.CarNumber;

            await _driverUow.SaveAsync();
            TempData["Success"] = "Driver updated successfully.";
            return RedirectToAction(nameof(DriverReports));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDriver(string id)
        {
            var driver = await _driverUow.Entity.GetAllAsyncAsQuery()
                .Include(d => d.appUsers)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (driver == null)
            {
                TempData["ErrorMessage"] = "Driver not found.";
                return RedirectToAction(nameof(DriverReports));
            }
            _driverUow.Entity.Delete(id);
            if (driver.appUsers != null)
            {
                _userUow.Entity.Delete(driver.appUsers.Id);
            }
            await _driverUow.SaveAsync();
            TempData["Success"] = "Driver deleted successfully.";
            return RedirectToAction(nameof(DriverReports));
        }

        [HttpGet]
        public async Task<IActionResult> EditStaff(string id)
        {
            var staff = await _staffUow.Entity.GetAllAsyncAsQuery()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (staff == null) return NotFound();

            var vm = new StaffEditViewModel
            {
                Id = staff.Id,
                UserId = staff.UserId,
                FirstName = staff.User?.FirstName ?? string.Empty,
                LastName = staff.User?.LastName ?? string.Empty,
                Email = staff.User?.Email ?? string.Empty,
                PhoneNumber = staff.User?.PhoneNumber,
                StationId = staff.StationId
            };

            ViewBag.Stations = await _stationUow.Entity.GetAllAsyncAsQuery().ToListAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaff(StaffEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Stations = await _stationUow.Entity.GetAllAsyncAsQuery().ToListAsync();
                return View(vm);
            }

            var staff = await _staffUow.Entity.GetAllAsyncAsQuery()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == vm.Id);
            if (staff == null) return NotFound();

            if (staff.User != null)
            {
                staff.User.FirstName = vm.FirstName;
                staff.User.LastName = vm.LastName;
                staff.User.Email = vm.Email;
                staff.User.UserName = vm.Email;
                staff.User.PhoneNumber = vm.PhoneNumber;
            }
            staff.StationId = vm.StationId;

            await _staffUow.SaveAsync();
            TempData["Success"] = "Staff updated successfully.";
            return RedirectToAction(nameof(StaffReports));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStaff(string id)
        {
            var staff = await _staffUow.Entity.GetAllAsyncAsQuery()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (staff == null)
            {
                TempData["ErrorMessage"] = "Staff not found.";
                return RedirectToAction(nameof(StaffReports));
            }

            // Remove staff profile and linked user
            _staffUow.Entity.Delete(id);
            if (staff.User != null)
            {
                _userUow.Entity.Delete(staff.User.Id);
            }
            await _staffUow.SaveAsync();

            TempData["Success"] = "Staff deleted successfully.";
            return RedirectToAction(nameof(StaffReports));
        }

        [HttpGet]
        public async Task<IActionResult> AddStaff()
        {
            try
            {
                var stations = await _stationUow.Entity.GetAllAsyncAsQuery().ToListAsync();
                ViewBag.Stations = stations;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading add staff page");
                TempData["ErrorMessage"] = "Error loading page. Please try again.";
                return RedirectToAction(nameof(ManagerHome));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStaff(AddStaffViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Stations = await _stationUow.Entity.GetAllAsyncAsQuery().ToListAsync();
                    return View(model);
                }

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "A user with this email already exists.");
                    ViewBag.Stations = await _stationUow.Entity.GetAllAsyncAsQuery().ToListAsync();
                    return View(model);
                }

                // Create new user
                var user = new AppUsers
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true // Auto-confirm for staff
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    ViewBag.Stations = await _stationUow.Entity.GetAllAsyncAsQuery().ToListAsync();
                    return View(model);
                }

                // Add staff role
                await _userManager.AddToRoleAsync(user, "Staff");

                // Create staff profile
                var staffProfile = new StaffProfile
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    StationId = model.StationId
                };

                await _staffUow.Entity.AddAsync(staffProfile);
                await _staffUow.SaveAsync();

                TempData["Success"] = "Staff member added successfully.";
                return RedirectToAction(nameof(StaffReports));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding staff member");
                TempData["ErrorMessage"] = "Error adding staff member. Please try again.";
                ViewBag.Stations = await _stationUow.Entity.GetAllAsyncAsQuery().ToListAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateDriver()
        {
            try
            {
                // Get all users who are not already drivers or staff
                var existingDriverIds = await _driverUow.Entity.GetAllAsyncAsQuery()
                    .Select(d => d.UserId)
                    .ToListAsync();

                var existingStaffIds = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Select(s => s.UserId)
                    .ToListAsync();

                var availableUsers = await _userManager.Users
                    .Where(u => !existingDriverIds.Contains(u.Id) && !existingStaffIds.Contains(u.Id))
                    .ToListAsync();

                var viewModel = new CreateDriverViewModel
                {
                    AvailableUsers = availableUsers.Select(u => new UserViewModel
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        UserName = u.UserName
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create driver page");
                TempData["ErrorMessage"] = "Error loading page. Please try again.";
                return RedirectToAction(nameof(ManagerHome));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDriver(CreateDriverViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Reload available users
                    var existingDriverIds = await _driverUow.Entity.GetAllAsyncAsQuery()
                        .Select(d => d.UserId)
                        .ToListAsync();

                    var existingStaffIds = await _staffUow.Entity.GetAllAsyncAsQuery()
                        .Select(s => s.UserId)
                        .ToListAsync();

                    var availableUsers = await _userManager.Users
                        .Where(u => !existingDriverIds.Contains(u.Id) && !existingStaffIds.Contains(u.Id))
                        .ToListAsync();

                    model.AvailableUsers = availableUsers.Select(u => new UserViewModel
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        UserName = u.UserName
                    }).ToList();

                    return View(model);
                }

                // Check if user already has a driver profile
                var existingDriver = await _driverUow.Entity.GetAllAsyncAsQuery()
                    .FirstOrDefaultAsync(d => d.UserId == model.UserId);

                if (existingDriver != null)
                {
                    ModelState.AddModelError("UserId", "This user is already a driver.");
                    return View(model);
                }

                // Create driver profile
                var driverProfile = new DriverProfile
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = model.UserId,
                    LicenseNumber = model.LicenseNumber,
                    CarNumber = model.CarNumber,
                    Lat = 0, // Default location
                    Long = 0
                };

                await _driverUow.Entity.AddAsync(driverProfile);

                // Add driver role to user
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    await _userManager.AddToRoleAsync(user, "Driver");
                }

                await _driverUow.SaveAsync();
                TempData["Success"] = "Driver created successfully.";
                return RedirectToAction(nameof(DriverReports));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating driver");
                TempData["ErrorMessage"] = "Error creating driver. Please try again.";
                return View(model);
            }
        }
    }
}
