using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Core.Interfaces;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Microsoft.AspNetCore.Identity;
using Route = Core.Entities.Route;
using Web.Models.Staff;
using Web.Models.Trip;

namespace Web.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        private readonly ILogger<StaffController> _logger;
        private readonly IUnitOfWork<StaffProfile> _staffUow;
        private readonly IUnitOfWork<Station> _stationUow;
        private readonly IUnitOfWork<DriverProfile> _driverUow;
        private readonly IUnitOfWork<AppUsers> _userUow;
        private readonly IUnitOfWork<Route> _routeUow;
        private readonly IUnitOfWork<Trip> _tripUow;
        private readonly IUnitOfWork<TripDriverQueue> _tripDriverQueueUow;
        private readonly UserManager<AppUsers> _userManager;

        public StaffController(
            ILogger<StaffController> logger,
            IUnitOfWork<StaffProfile> staffUow,
            IUnitOfWork<Station> stationUow,
            IUnitOfWork<DriverProfile> driverUow,
            IUnitOfWork<AppUsers> userUow,
            IUnitOfWork<Route> routeUow,
            IUnitOfWork<Trip> tripUow,
            IUnitOfWork<TripDriverQueue> tripDriverQueueUow,
            UserManager<AppUsers> userManager)
        {
            _logger = logger;
            _staffUow = staffUow;
            _stationUow = stationUow;
            _driverUow = driverUow;
            _userUow = userUow;
            _routeUow = routeUow;
            _tripUow = tripUow;
            _tripDriverQueueUow = tripDriverQueueUow;
            _userManager = userManager;
        }

        public async Task<IActionResult> Home()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }

                // Get staff profile with station information
                var staffProfile = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Include(s => s.User)
                    .Include(s => s.Station)
                    .FirstOrDefaultAsync(s => s.UserId == currentUserId);

                if (staffProfile == null)
                {
                    TempData["ErrorMessage"] = "Staff profile not found.";
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }

                // Get station drivers count (drivers who have trips from this station)
                var stationDriversCount = await _driverUow.Entity.GetAllAsyncAsQuery()
                    .Include(d => d.Trips)
                        .ThenInclude(t => t.Route)
                    .Where(d => d.Trips.Any(t => t.Route.StartStationId == staffProfile.StationId))
                    .CountAsync();

                // Get upcoming trips from this station
                var upcomingTrips = await _tripUow.Entity.GetAllAsyncAsQuery()
                    .Include(t => t.Route)
                        .ThenInclude(r => r.StartStation)
                    .Include(t => t.Route)
                        .ThenInclude(r => r.EndStation)
                    .Include(t => t.Driver)
                        .ThenInclude(d => d.appUsers)
                    .Where(t => t.Route.StartStationId == staffProfile.StationId && t.DepartureTime > DateTime.Now)
                    .OrderBy(t => t.DepartureTime)
                    .Take(5)
                    .ToListAsync();

                var viewModel = new StaffHomeViewModel
                {
                    StaffName = $"{staffProfile.User?.FirstName} {staffProfile.User?.LastName}".Trim(),
                    StationName = staffProfile.Station?.Name ?? "Unknown Station",
                    StationCity = staffProfile.Station?.City ?? "Unknown City",
                    StationId = staffProfile.StationId,
                    DriversCount = stationDriversCount,
                    UpcomingTrips = upcomingTrips.Select(t => new TripSummaryViewModel
                    {
                        Id = t.Id,
                        RouteName = $"{t.Route?.StartStation?.Name} → {t.Route?.EndStation?.Name}",
                        DepartureTime = t.DepartureTime,
                        AvailableSeats = t.AvailableSeats,
                        DriverName = $"{t.Driver?.appUsers?.FirstName} {t.Driver?.appUsers?.LastName}".Trim(),
                        Price = t.Route?.Price ?? 0
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading staff home page");
                TempData["ErrorMessage"] = "Error loading home page. Please try again.";
                return View(new StaffHomeViewModel());
            }
        }

        public async Task<IActionResult> Profile()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }

                var staffProfile = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Include(s => s.User)
                    .Include(s => s.Station)
                    .FirstOrDefaultAsync(s => s.UserId == currentUserId);

                if (staffProfile == null)
                {
                    TempData["ErrorMessage"] = "Staff profile not found.";
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }

                var viewModel = new StaffProfileViewModel
                {
                    Id = staffProfile.Id,
                    UserId = staffProfile.UserId,
                    FirstName = staffProfile.User?.FirstName ?? "",
                    LastName = staffProfile.User?.LastName ?? "",
                    Email = staffProfile.User?.Email ?? "",
                    PhoneNumber = staffProfile.User?.PhoneNumber,
                    StationId = staffProfile.StationId,
                    StationName = staffProfile.Station?.Name ?? "Unknown",
                    StationCity = staffProfile.Station?.City ?? "Unknown"
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading staff profile");
                TempData["ErrorMessage"] = "Error loading profile. Please try again.";
                return RedirectToAction("Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(StaffProfileViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Profile", model);
                }

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var staffProfile = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.UserId == currentUserId);

                if (staffProfile == null)
                {
                    TempData["ErrorMessage"] = "Staff profile not found.";
                    return RedirectToAction("Profile");
                }

                if (staffProfile.User != null)
                {
                    staffProfile.User.FirstName = model.FirstName;
                    staffProfile.User.LastName = model.LastName;
                    staffProfile.User.Email = model.Email;
                    staffProfile.User.UserName = model.Email;
                    staffProfile.User.PhoneNumber = model.PhoneNumber;
                }

                await _staffUow.SaveAsync();
                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff profile");
                TempData["ErrorMessage"] = "Error updating profile. Please try again.";
                return View("Profile", model);
            }
        }

        public async Task<IActionResult> AddDriver()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var staffProfile = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Include(s => s.Station)
                    .FirstOrDefaultAsync(s => s.UserId == currentUserId);

                if (staffProfile == null)
                {
                    TempData["ErrorMessage"] = "Staff profile not found.";
                    return RedirectToAction("Home");
                }

                // Get all users who are not already drivers
                var existingDriverIds = await _driverUow.Entity.GetAllAsyncAsQuery()
                    .Select(d => d.UserId)
                    .ToListAsync();

                var availableUsers = await _userManager.Users
                    .Where(u => !existingDriverIds.Contains(u.Id))
                    .ToListAsync();

                var viewModel = new AddDriverViewModel
                {
                    StationId = staffProfile.StationId,
                    StationName = staffProfile.Station?.Name ?? "Unknown",
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
                _logger.LogError(ex, "Error loading add driver page");
                TempData["ErrorMessage"] = "Error loading page. Please try again.";
                return RedirectToAction("Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddDriverByCarNumber()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var staffProfile = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Include(s => s.Station)
                    .FirstOrDefaultAsync(s => s.UserId == currentUserId);

                if (staffProfile == null)
                {
                    TempData["ErrorMessage"] = "Staff profile not found.";
                    return RedirectToAction("Home");
                }

                var vm = new AddDriverByCarNumberViewModel
                {
                    StationName = staffProfile.Station?.Name ?? "Unknown"
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading add driver by car number page");
                TempData["ErrorMessage"] = "Error loading page. Please try again.";
                return RedirectToAction("Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDriverByCarNumber(AddDriverByCarNumberViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var staffProfile = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Include(s => s.Station)
                    .FirstOrDefaultAsync(s => s.UserId == currentUserId);

                if (staffProfile == null)
                {
                    TempData["ErrorMessage"] = "Staff profile not found.";
                    return RedirectToAction("Home");
                }

                var driver = await _driverUow.Entity.GetAllAsyncAsQuery()
                    .FirstOrDefaultAsync(d => d.CarNumber == model.CarNumber);

                if (driver == null)
                {
                    ModelState.AddModelError("CarNumber", "No driver found with this car number.");
                    return View(model);
                }

                // No station assignment table exists; treat this as presence validation
                TempData["SuccessMessage"] = "Driver found and can be assigned to trips from your station.";
                return RedirectToAction("CreateTrip");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding driver by car number");
                TempData["ErrorMessage"] = "Error adding driver. Please try again.";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDriver(AddDriverViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
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
                TempData["SuccessMessage"] = "Driver added successfully.";
                return RedirectToAction("ListDrivers");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding driver");
                TempData["ErrorMessage"] = "Error adding driver. Please try again.";
                return View(model);
            }
        }

        public async Task<IActionResult> ListDrivers()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var staffProfile = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Include(s => s.Station)
                    .FirstOrDefaultAsync(s => s.UserId == currentUserId);

                if (staffProfile == null)
                {
                    TempData["ErrorMessage"] = "Staff profile not found.";
                    return RedirectToAction("Home");
                }

                // Get drivers who have trips from this station
                var drivers = await _driverUow.Entity.GetAllAsyncAsQuery()
                    .Include(d => d.appUsers)
                    .Include(d => d.Trips)
                        .ThenInclude(t => t.Route)
                    .Where(d => d.Trips.Any(t => t.Route.StartStationId == staffProfile.StationId))
                    .ToListAsync();

                var viewModel = new ListDriversViewModel
                {
                    StationName = staffProfile.Station?.Name ?? "Unknown",
                    Drivers = drivers.Select(d => new DriverViewModel
                    {
                        Id = d.Id,
                        UserId = d.UserId,
                        FirstName = d.appUsers?.FirstName ?? "Unknown",
                        LastName = d.appUsers?.LastName ?? "Unknown",
                        Email = d.appUsers?.Email ?? "Unknown",
                        LicenseNumber = d.LicenseNumber,
                        CarNumber = d.CarNumber,
                        TripsCount = d.Trips?.Count ?? 0
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading drivers list");
                TempData["ErrorMessage"] = "Error loading drivers. Please try again.";
                return RedirectToAction("Home");
            }
        }

        public async Task<IActionResult> CreateTrip()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var staffProfile = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Include(s => s.Station)
                    .FirstOrDefaultAsync(s => s.UserId == currentUserId);

                if (staffProfile == null)
                {
                    TempData["ErrorMessage"] = "Staff profile not found.";
                    return RedirectToAction("Home");
                }

                // Get routes that start from this station
                var routes = await _routeUow.Entity.GetAllAsyncAsQuery()
                    .Include(r => r.StartStation)
                    .Include(r => r.EndStation)
                    .Where(r => r.StartStationId == staffProfile.StationId)
                    .ToListAsync();

                var viewModel = new CreateTripWithQueueViewModel
                {
                    StationName = staffProfile.Station?.Name ?? "Unknown",
                    Routes = routes.Select(r => new RouteViewModel
                    {
                        Id = r.Id,
                        Name = $"{r.StartStation?.Name} → {r.EndStation?.Name}",
                        Price = r.Price
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create trip page");
                TempData["ErrorMessage"] = "Error loading page. Please try again.";
                return RedirectToAction("Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrip(CreateTripWithQueueViewModel model)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var staffProfile = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Include(s => s.Station)
                    .FirstOrDefaultAsync(s => s.UserId == currentUserId);

                if (staffProfile == null)
                {
                    TempData["ErrorMessage"] = "Staff profile not found.";
                    return RedirectToAction("Home");
                }

                if (!ModelState.IsValid)
                {
                    // Repopulate lists
                    var routesAll = await _routeUow.Entity.GetAllAsyncAsQuery()
                        .Include(r => r.StartStation)
                        .Include(r => r.EndStation)
                        .Where(r => r.StartStationId == staffProfile.StationId)
                        .ToListAsync();

                    model.Routes = routesAll.Select(r => new RouteViewModel
                    {
                        Id = r.Id,
                        Name = $"{r.StartStation?.Name} → {r.EndStation?.Name}",
                        Price = r.Price
                    }).ToList();

                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.PrimaryDriverCarNumber))
                {
                    ModelState.AddModelError("PrimaryDriverCarNumber", "Please enter a driver car number.");
                    return View(model);
                }

                // Validate departure time is in the future
                if (model.DepartureTime <= DateTime.Now)
                {
                    ModelState.AddModelError("DepartureTime", "Departure time must be in the future.");

                    // Repopulate lists
                    var routesAll = await _routeUow.Entity.GetAllAsyncAsQuery()
                        .Include(r => r.StartStation)
                        .Include(r => r.EndStation)
                        .Where(r => r.StartStationId == staffProfile.StationId)
                        .ToListAsync();

                    model.Routes = routesAll.Select(r => new RouteViewModel
                    {
                        Id = r.Id,
                        Name = $"{r.StartStation?.Name} → {r.EndStation?.Name}",
                        Price = r.Price
                    }).ToList();

                    return View(model);
                }

                // Validate the selected route starts from staff station
                var selectedRoute = await _routeUow.Entity.GetAllAsyncAsQuery().FirstOrDefaultAsync(r => r.Id == model.RouteId);
                if (selectedRoute == null || selectedRoute.StartStationId != staffProfile.StationId)
                {
                    ModelState.AddModelError("RouteId", "Selected route must start from your station.");

                    // Repopulate lists
                    var routesAll = await _routeUow.Entity.GetAllAsyncAsQuery()
                        .Include(r => r.StartStation)
                        .Include(r => r.EndStation)
                        .Where(r => r.StartStationId == staffProfile.StationId)
                        .ToListAsync();

                    model.Routes = routesAll.Select(r => new RouteViewModel
                    {
                        Id = r.Id,
                        Name = $"{r.StartStation?.Name} → {r.EndStation?.Name}",
                        Price = r.Price
                    }).ToList();

                    return View(model);
                }

                // Find primary driver by car number
                var primaryDriver = await _driverUow.Entity.GetAllAsyncAsQuery()
                    .FirstOrDefaultAsync(d => d.CarNumber == model.PrimaryDriverCarNumber);
                if (primaryDriver == null)
                {
                    ModelState.AddModelError("PrimaryDriverCarNumber", "No driver found with this car number.");

                    var routesAll = await _routeUow.Entity.GetAllAsyncAsQuery()
                        .Include(r => r.StartStation)
                        .Include(r => r.EndStation)
                        .Where(r => r.StartStationId == staffProfile.StationId)
                        .ToListAsync();
                    model.Routes = routesAll.Select(r => new RouteViewModel
                    {
                        Id = r.Id,
                        Name = $"{r.StartStation?.Name} → {r.EndStation?.Name}",
                        Price = r.Price
                    }).ToList();
                    return View(model);
                }

                // Create the trip with the located driver as primary
                var trip = new Trip
                {
                    RouteId = model.RouteId,
                    DriverId = primaryDriver.Id,
                    DepartureTime = model.DepartureTime,
                    AvailableSeats = model.AvailableSeats
                };

                await _tripUow.Entity.AddAsync(trip);
                await _tripUow.SaveAsync();

                // Assign only the primary driver in queue as Assigned
                var driverQueue = new TripDriverQueue
                {
                    TripId = trip.Id,
                    DriverId = primaryDriver.Id,
                    QueueOrder = 1,
                    Status = DriverStatus.Assigned,
                    AssignedAt = DateTime.Now
                };
                await _tripDriverQueueUow.Entity.AddAsync(driverQueue);
                await _tripDriverQueueUow.SaveAsync();

                TempData["SuccessMessage"] = "Trip created successfully and primary driver assigned.";
                return RedirectToAction("Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trip");
                TempData["ErrorMessage"] = "Error creating trip. Please try again.";
                return View(model);
            }
        }

        public async Task<IActionResult> ViewTrips()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var staffProfile = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Include(s => s.Station)
                    .FirstOrDefaultAsync(s => s.UserId == currentUserId);

                if (staffProfile == null)
                {
                    TempData["ErrorMessage"] = "Staff profile not found.";
                    return RedirectToAction("Home");
                }

                var trips = await _tripUow.Entity.GetAllAsyncAsQuery()
                    .Include(t => t.Route)
                        .ThenInclude(r => r.StartStation)
                    .Include(t => t.Route)
                        .ThenInclude(r => r.EndStation)
                    .Include(t => t.Driver)
                        .ThenInclude(d => d.appUsers)
                    .Include(t => t.Bookings)
                    .Where(t => t.Route.StartStationId == staffProfile.StationId)
                    .OrderByDescending(t => t.DepartureTime)
                    .ToListAsync();

                var viewModel = new ViewTripsViewModel
                {
                    StationName = staffProfile.Station?.Name ?? "Unknown",
                    Trips = trips.Select(t => new TripDetailsViewModel
                    {
                        Id = t.Id,
                        RouteName = $"{t.Route?.StartStation?.Name} → {t.Route?.EndStation?.Name}",
                        DepartureTime = t.DepartureTime,
                        AvailableSeats = t.AvailableSeats,
                        DriverName = $"{t.Driver?.appUsers?.FirstName} {t.Driver?.appUsers?.LastName}".Trim(),
                        Price = t.Route?.Price ?? 0,
                        BookingsCount = t.Bookings?.Count ?? 0,
                        IsUpcoming = t.DepartureTime > DateTime.Now
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading trips");
                TempData["ErrorMessage"] = "Error loading trips. Please try again.";
                return RedirectToAction("Home");
            }
        }

        public async Task<IActionResult> ViewTripDriverQueue(string tripId)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var staffProfile = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Include(s => s.Station)
                    .FirstOrDefaultAsync(s => s.UserId == currentUserId);

                if (staffProfile == null)
                {
                    TempData["ErrorMessage"] = "Staff profile not found.";
                    return RedirectToAction("Home");
                }

                var trip = await _tripUow.Entity.GetAllAsyncAsQuery()
                    .Include(t => t.Route)
                        .ThenInclude(r => r.StartStation)
                    .Include(t => t.Route)
                        .ThenInclude(r => r.EndStation)
                    .Include(t => t.Driver)
                        .ThenInclude(d => d.appUsers)
                    .FirstOrDefaultAsync(t => t.Id == tripId);

                if (trip == null || trip.Route?.StartStationId != staffProfile.StationId)
                {
                    TempData["ErrorMessage"] = "Trip not found or not accessible.";
                    return RedirectToAction("ViewTrips");
                }

                var driverQueue = await _tripDriverQueueUow.Entity.GetAllAsyncAsQuery()
                    .Include(q => q.Driver)
                        .ThenInclude(d => d.appUsers)
                    .Where(q => q.TripId == tripId)
                    .OrderBy(q => q.QueueOrder)
                    .ToListAsync();

                var viewModel = new
                {
                    Trip = new
                    {
                        Id = trip.Id,
                        RouteName = $"{trip.Route?.StartStation?.Name} → {trip.Route?.EndStation?.Name}",
                        DepartureTime = trip.DepartureTime,
                        AvailableSeats = trip.AvailableSeats,
                        PrimaryDriver = $"{trip.Driver?.appUsers?.FirstName} {trip.Driver?.appUsers?.LastName}".Trim()
                    },
                    DriverQueue = driverQueue.Select(q => new TripDriverQueueViewModel
                    {
                        Id = q.Id,
                        TripId = q.TripId,
                        DriverId = q.DriverId,
                        DriverName = $"{q.Driver?.appUsers?.FirstName} {q.Driver?.appUsers?.LastName}".Trim(),
                        CarNumber = q.Driver?.CarNumber ?? "",
                        QueueOrder = q.QueueOrder,
                        Status = q.Status,
                        AssignedAt = q.AssignedAt,
                        StartedAt = q.StartedAt,
                        CompletedAt = q.CompletedAt
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading trip driver queue");
                TempData["ErrorMessage"] = "Error loading driver queue. Please try again.";
                return RedirectToAction("ViewTrips");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDriverStatus(string queueId, DriverStatus newStatus)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var staffProfile = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Include(s => s.Station)
                    .FirstOrDefaultAsync(s => s.UserId == currentUserId);

                if (staffProfile == null)
                {
                    return Json(new { success = false, message = "Staff profile not found." });
                }

                var driverQueue = await _tripDriverQueueUow.Entity.GetAllAsyncAsQuery()
                    .Include(q => q.Trip)
                        .ThenInclude(t => t.Route)
                    .FirstOrDefaultAsync(q => q.Id == queueId);

                if (driverQueue == null || driverQueue.Trip?.Route?.StartStationId != staffProfile.StationId)
                {
                    return Json(new { success = false, message = "Driver queue not found or not accessible." });
                }

                // Update driver status
                driverQueue.Status = newStatus;
                
                switch (newStatus)
                {
                    case DriverStatus.Assigned:
                        driverQueue.AssignedAt = DateTime.Now;
                        break;
                    case DriverStatus.Started:
                        driverQueue.StartedAt = DateTime.Now;
                        break;
                    case DriverStatus.Completed:
                        driverQueue.CompletedAt = DateTime.Now;
                        break;
                }

                await _tripDriverQueueUow.SaveAsync();

                // Check if all drivers have completed or cancelled
                var allDrivers = await _tripDriverQueueUow.Entity.GetAllAsyncAsQuery()
                    .Where(q => q.TripId == driverQueue.TripId)
                    .ToListAsync();

                var allCompleted = allDrivers.All(d => d.Status == DriverStatus.Completed || d.Status == DriverStatus.Cancelled);
                
                if (allCompleted)
                {
                    // Close the trip - you might want to add a status field to Trip entity
                    TempData["InfoMessage"] = "All drivers have completed the trip. Trip is now closed.";
                }

                return Json(new { success = true, message = "Driver status updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating driver status");
                return Json(new { success = false, message = "Error updating driver status." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteNextDriver(string tripId)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var staffProfile = await _staffUow.Entity.GetAllAsyncAsQuery()
                    .Include(s => s.Station)
                    .FirstOrDefaultAsync(s => s.UserId == currentUserId);

                if (staffProfile == null)
                {
                    return Json(new { success = false, message = "Staff profile not found." });
                }

                var trip = await _tripUow.Entity.GetAllAsyncAsQuery()
                    .Include(t => t.Route)
                    .FirstOrDefaultAsync(t => t.Id == tripId);

                if (trip == null || trip.Route?.StartStationId != staffProfile.StationId)
                {
                    return Json(new { success = false, message = "Trip not found or not accessible." });
                }

                // Find the next available driver in queue
                var nextDriver = await _tripDriverQueueUow.Entity.GetAllAsyncAsQuery()
                    .Include(q => q.Driver)
                    .Where(q => q.TripId == tripId && q.Status == DriverStatus.Queued)
                    .OrderBy(q => q.QueueOrder)
                    .FirstOrDefaultAsync();

                if (nextDriver == null)
                {
                    return Json(new { success = false, message = "No more drivers available in queue." });
                }

                // Update current assigned driver to cancelled
                var currentAssigned = await _tripDriverQueueUow.Entity.GetAllAsyncAsQuery()
                    .Where(q => q.TripId == tripId && q.Status == DriverStatus.Assigned)
                    .FirstOrDefaultAsync();

                if (currentAssigned != null)
                {
                    currentAssigned.Status = DriverStatus.Cancelled;
                }

                // Promote next driver
                nextDriver.Status = DriverStatus.Assigned;
                nextDriver.AssignedAt = DateTime.Now;

                // Update trip's primary driver
                trip.DriverId = nextDriver.DriverId;

                await _tripUow.SaveAsync();
                await _tripDriverQueueUow.SaveAsync();

                return Json(new { 
                    success = true, 
                    message = $"Driver {nextDriver.Driver?.appUsers?.FirstName} {nextDriver.Driver?.appUsers?.LastName} has been promoted to primary driver." 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error promoting next driver");
                return Json(new { success = false, message = "Error promoting next driver." });
            }
        }
    }
}
