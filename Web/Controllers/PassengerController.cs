using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Services;
using System.Security.Claims;
using Web.Models;
using Web.Models.Booking;
using Web.Models.Trip;
using Web.Models.Account;
using Web.Models.History;
using Core.Enums;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;



namespace Web.Controllers
{
    [Authorize(Roles = "Passenger,Staff")]
    public class PassengerController : Controller
    {

        private readonly IBookingService _bookingService;
        private readonly ITripService _tripService;
        private readonly IPassengerHelperService _passengerHelperService;
        private readonly IUnitOfWork<Station> _stationUow;
        private readonly ILogger<PassengerController> _logger;
        private readonly UserManager<AppUsers> _userManager;

        public PassengerController(
            IBookingService bookingService,
            ITripService tripService,
            IPassengerHelperService passengerHelperService,
             IUnitOfWork<Station> stationUow, ILogger<PassengerController> _logger,
             UserManager<AppUsers> userManager)
        {
            _bookingService = bookingService;
            _tripService = tripService;
            _passengerHelperService = passengerHelperService;
            _stationUow = stationUow;
            this._logger = _logger;
            _userManager = userManager;
        }

        // Helper method to get current passenger ID
        private async Task<string?> GetCurrentPassengerIdAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return null;

            var passengerId = await _passengerHelperService.GetPassengerIdFromUserIdAsync(userId);
            
            // If no passenger profile exists, create one automatically
            if (passengerId == null)
            {
                var passengerProfile = new PassengerProfile
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId
                };

                var passengerUow = HttpContext.RequestServices.GetRequiredService<IUnitOfWork<PassengerProfile>>();
                passengerUow.Entity.Insert(passengerProfile);
                passengerUow.Save();
                
                return passengerProfile.Id;
            }

            return passengerId;
        }

        #region Booking Page
        [HttpGet]
        public async Task<IActionResult> Booking()
        {
            try
            {

                
                // Fetch all trips that start from stations where City = 'المنصورة'
                var mansouraTrips = await _stationUow.Entity
                    .GetAllAsyncAsQuery()
                    .Where(s => s.City == "المنصورة")
                    .SelectMany(s => s.RoutesFrom)
                    .SelectMany(r => r.Trips)
                    .Where(t => t.AvailableSeats > 0)
                    .Include(t => t.Route)
                        .ThenInclude(r => r.StartStation)
                    .Include(t => t.Route)
                        .ThenInclude(r => r.EndStation)
                    .Include(t => t.Driver)
                        .ThenInclude(d => d.appUsers)
                    .Include(t => t.Bookings)
                    .ToListAsync();

                // Create a list of trip data for the view
                var stationData = new List<object>();
                
                foreach (var trip in mansouraTrips)
                {
                    var bookedSeats = trip.Bookings?.Count(b => b.Status != BookingStatus.Cancelled) ?? 0;
                    var availableSeats = trip.AvailableSeats - bookedSeats;
                    
                    if (availableSeats > 0)
                    {
                        stationData.Add(new
                        {
                            StationId = trip.Route?.StartStation?.Id,
                            StationName = trip.Route?.StartStation?.Name,
                            City = trip.Route?.StartStation?.City,
                            TripId = trip.Id,
                            BusNumber = $"Bus #{trip.Id.Substring(0, Math.Min(3, trip.Id.Length))}",
                            AvailableSeats = availableSeats,
                            Price = trip.Route?.Price ?? 0,
                            Route = $"{trip.Route?.EndStation?.Name} → {trip.Route?.StartStation?.Name}",
                            DriverName = $"{trip.Driver?.appUsers?.FirstName} {trip.Driver?.appUsers?.LastName}",
                            CarNumber = trip.Driver?.CarNumber ?? "N/A",
                            // Default coordinates for Mansoura area
                            Latitude = 31.0409,
                            Longitude = 31.3785
                        });
                    }
                }

                ViewBag.StationData = stationData;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading booking page data");
                ViewBag.StationData = new List<object>();
                return View();
            }
        }
        #endregion

        #region Search for trips
        [HttpGet]
        public async Task<IActionResult> SearchTrips(string? OriginStationId = null, string? DestinationStationId = null)
        {
            var model = new SearchTripsViewModel();

            var stations = await _stationUow.Entity.GetAllAsyncAsQuery().ToListAsync();
            model.Stations = stations.Select(s => new SelectListItem
            {
                Value = s.Id,
                Text = $"{s.Name} - {s.City}"
            }).ToList();


            if (!string.IsNullOrEmpty(OriginStationId) && !string.IsNullOrEmpty(DestinationStationId))
            {
                model.OriginStationId = OriginStationId;
                model.DestinationStationId = DestinationStationId;

               
                try
                {
                    var trips = await _tripService.SearchTripsAsync(
                        OriginStationId,
                        DestinationStationId
                    );

                    model.SearchResults = trips.Select(trip => new TripResultViewModel
                    {
                        TripId = trip.Id,
                        OriginStation = trip.Route?.StartStation?.Name!,
                        DestinationStation = trip.Route?.EndStation?.Name!,
                        Price = trip.Route?.Price ?? 0,
                        AvailableSeats = trip.AvailableSeats - (trip.Bookings?.Count(b => b.Status != BookingStatus.Cancelled) ?? 0),
                        DriverName = $"{trip.Driver?.appUsers?.FirstName} {trip.Driver?.appUsers?.LastName}",
                        CarNumber = trip.Driver?.CarNumber!
                    }).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error searching trips from home page");
                    ModelState.AddModelError("", "Something Going Wrong");
                }
            }


            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> SearchTrips(SearchTripsViewModel model)
        {
            Console.WriteLine($"Model values - From: '{model.OriginStationId}', To: '{model.DestinationStationId}'");

            if (!ModelState.IsValid)
            {
                var stations = await _stationUow.Entity.GetAllAsyncAsQuery().ToListAsync();
                model.Stations = stations.Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = $"{s.Name} - {s.City}"
                }).ToList();

                return View(model);

            }

            try
            {
                Console.WriteLine("Calling SearchTripsAsync...");

                var trips = await _tripService.SearchTripsAsync(
                    model.OriginStationId,
                    model.DestinationStationId
                   );

                Console.WriteLine($"Found {trips.Count()} trips");

                model.SearchResults = trips.Select(trip => new TripResultViewModel
                {
                    TripId = trip.Id,
                   
                    OriginStation = trip.Route?.StartStation?.Name!,
                    DestinationStation = trip.Route?.EndStation?.Name!,
                    Price = trip.Route?.Price ?? 0,
                    AvailableSeats = trip.AvailableSeats - (trip.Bookings?.Count(b => b.Status != BookingStatus.Cancelled) ?? 0),
                    DriverName = $"{trip.Driver?.appUsers?.FirstName} {trip.Driver?.appUsers?.LastName}",
                    CarNumber = trip.Driver?.CarNumber!
                }).ToList();

                return View(model);
            }

            catch (Exception ex) {
                Console.WriteLine($"Exception occurred: {ex.Message}");

                ModelState.AddModelError("", "Something going wrong");
                return View(model);
            }

        }
        #endregion


        #region Trip Details

        [HttpGet]
        public async Task<IActionResult> TripDetails(string id)
        { 
        
            var trip = await _tripService.GetTripDetailsAsync(id);
            if (trip == null)
            {
                TempData["ErrorMessage"] = "Trip Not Found";
                return RedirectToAction("Booking");
            }

            var bookedSeats = trip.Bookings?.Count(b => b.Status != BookingStatus.Cancelled) ?? 0;
            var passengerId = await GetCurrentPassengerIdAsync();
            
            var model = new TripDetailsViewModel
            {
                TripId = trip.Id,
                DepartureTime = trip.DepartureTime,
                OriginStation = new StationViewModel
                {
                    Id = trip.Route?.StartStation?.Id ?? "0",
                    Name = trip.Route?.StartStation?.Name ?? string.Empty,
                    City = trip.Route?.StartStation?.City!

                },
                DestinationStation = new StationViewModel
                {
                    Id = trip.Route?.EndStation?.Id ?? "0",
                    Name = trip.Route?.EndStation?.Name ?? string.Empty
                },
                Price = trip.Route?.Price ?? 0,
                TotalSeats = trip.AvailableSeats,
                BookedSeats = bookedSeats,
                AvailableSeats = trip.AvailableSeats - bookedSeats,
                IsUserLoggedIn = passengerId != null,
                Driver = new DriverViewModel
                {
                    Id = trip.Driver?.Id ?? "0",
                    Name = $"{trip.Driver?.appUsers?.FirstName} {trip.Driver?.appUsers?.LastName}",
                    CarNumber = trip.Driver?.CarNumber ?? string.Empty,
                    Phone = trip.Driver?.appUsers?.PhoneNumber ?? string.Empty
                }
            };

            return View(model);
        
        }

        #endregion


        #region Book a seat

        [HttpPost]
        public async Task<IActionResult> BookSeat(string tripId)
        {
            try
            {
                var passengerId = await GetCurrentPassengerIdAsync();
                if (passengerId == null)
                {
                    TempData["ErrorMessage"] = "Must LogIn First";
                    return RedirectToPage("/Account/Login", new { returnUrl = Url.Action("TripDetails", new { id = tripId }) });
                }

                var booking = await _bookingService.CreateBookingAsync(passengerId.ToString()!, tripId);
                _logger.LogInformation("Booking created successfully. BookingId: {BookingId}", booking.Id);
                TempData["SuccessMessage"] = "The seat has been successfully booked.";
                /// Redirect to payment page
                return RedirectToAction("Index","Payment", new { bookingId = booking.Id });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while booking seat for TripId: {TripId}", tripId);
                TempData["ErrorMessage"] = "An error occurred while booking the seat";
                return RedirectToAction("TripDetails", new { id = tripId });
            }

        }


        #endregion


        #region Bookings Details
        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            try
            {
                var passengerId = await GetCurrentPassengerIdAsync();
                if (passengerId == null)
                {
                    TempData["ErrorMessage"] = "Must LogIn Frist";
                    return RedirectToPage("/Account/Login");
                }

                var bookings = await _bookingService.GetPassengerBookingsAsync(passengerId.ToString()!);

                var model = new MyBookingsViewModel
                {
                    Bookings = bookings.Select(booking => new BookingViewModel
                    {
                        BookingId = booking.Id,
                        TripId = booking.TripId,
                        BookingDate = booking.BookingDate,
                        Status = booking.Status,
                        StatusText = GetBookingStatusText(booking.Status),

                        Trip = new TripSummaryViewModel
                        {
                         
                            OriginStation = booking.Trip?.Route?.StartStation?.Name,
                            DestinationStation = booking.Trip?.Route?.EndStation?.Name,
                            Price = booking.Trip?.Route?.Price ?? 0,
                            DriverName = $"{booking.Trip?.Driver?.appUsers?.FirstName} {booking.Trip?.Driver?.appUsers?.LastName}",
                            CarNumber = booking.Trip?.Driver?.CarNumber
                        },
                      //  PaymentStatus = GetPaymentStatus(booking.Payments),
                        CanCancel = CanCancelBooking(booking)
                    }).OrderByDescending(b => b.BookingDate).ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something Go Wrong";
                return View(new MyBookingsViewModel());
            }
        }

      
        #endregion


        #region Active Bookings 


        [HttpGet]
        public async Task<IActionResult> ActiveBookings()
        {
            try
            {
                var passengerId = await GetCurrentPassengerIdAsync();
                if (passengerId == null)
                {
                    TempData["ErrorMessage"] = "Must LogIn Frist";
                    return RedirectToPage("/Account/Login");
                }

                var bookings = await _bookingService.GetPassengerActiveBookingsAsync(passengerId.ToString()!);

                var model = new ActiveBookingsViewModel
                {
                    ActiveBookings = bookings.Select(booking => new ActiveBookingViewModel
                    {
                        BookingId = booking.Id,
                        TripId = booking.TripId,
                        BookingDate = booking.BookingDate,
                        Status = booking.Status,   
                        StatusText = GetBookingStatusText(booking.Status),
                        Trip = new TripSummaryViewModel
                        {
                            DepartureTime = booking.Trip?.DepartureTime ?? DateTime.MinValue,
                            OriginStation = booking.Trip?.Route?.StartStation?.Name!,
                            DestinationStation = booking.Trip?.Route?.EndStation?.Name!,
                            Price = booking.Trip?.Route?.Price ?? 0,
                            DriverName = $"{booking.Trip?.Driver?.appUsers?.FirstName} {booking.Trip?.Driver?.appUsers?.LastName}",
                            CarNumber = booking.Trip?.Driver?.CarNumber!,
                            DriverPhone = booking.Trip?.Driver?.appUsers?.PhoneNumber!
                        },
                     //   PaymentStatus = GetPaymentStatus(booking.Payments),
                        CanCancel = CanCancelBooking(booking),
                       
                    }).OrderBy(b => b.Trip.DepartureTime).ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something Go Wrong";
                return View(new ActiveBookingsViewModel());
            }

            

        }
        #endregion


        #region Booking Details
        [HttpGet]
        public async Task<IActionResult> BookingDetails(string id)
        {
            try
            {
                var booking = await _bookingService.GetBookingDetailsAsync(id);

                if (booking == null)
                {
                    TempData["ErrorMessage"] = "Booking Not Found";
                    return RedirectToAction("MyBookings");
                }

                // Verify that this booking belongs to the current passenger
                var passengerId = await GetCurrentPassengerIdAsync();
                if (passengerId == null || booking.PassengerId != passengerId)
                {
                    TempData["ErrorMessage"] = "You are Forbiden";
                    return RedirectToAction("MyBookings");
                }

                var model = new BookingDetailsViewModel
                {
                    BookingId =booking.Id,
                    TripId = booking.TripId,
                    BookingDate = booking.BookingDate,
                    Status = booking.Status,
                    StatusText = GetBookingStatusText(booking.Status),
                    Trip = new TripDetailsViewModel
                    {
                        TripId = booking.Trip.Id,
                        DepartureTime = booking.Trip.DepartureTime,
                        OriginStation = new StationViewModel
                        {
                            Name = booking.Trip?.Route?.StartStation?.Name,
                            City = booking.Trip?.Route?.StartStation?.City
                        },
                        DestinationStation = new StationViewModel
                        {
                            Name = booking.Trip?.Route?.EndStation?.Name,
                            City = booking.Trip?.Route?.EndStation?.City
                        },
                        Price = booking.Trip?.Route?.Price ?? 0,
                        Driver = new DriverViewModel
                        {
                            Name = $"{booking.Trip?.Driver?.appUsers?.FirstName} {booking.Trip?.Driver?.appUsers?.LastName}",
                            CarNumber = booking.Trip?.Driver?.CarNumber,
                            Phone = booking.Trip?.Driver?.appUsers?.PhoneNumber
                        }
                    },
                    //Payments = booking.Payments?.Select(p => new PaymentViewModel
                    //{
                    //    PaymentId = p.Id,
                    //    Amount = p.Amount,
                    //    Currency = p.Currency,
                    //    Status = p.Status,
                    //    StatusText = GetPaymentStatusText(p.Status),
                    //    Method = p.Method,
                    //    MethodText = GetPaymentMethodText(p.Method),
                    //    CreatedAt = p.CreatedAt,
                    //    TransactionReference = p.TransactionReference
                    //}).ToList() ?? new List<PaymentViewModel>(),
                    CanCancel = CanCancelBooking(booking),
                    CanConfirm = booking.Status == BookingStatus.Pending
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء عرض تفاصيل الحجز";
                return RedirectToAction("MyBookings");
            }
        }

        #endregion

        #region Booking Cancelation 
        [HttpPost]
        public async Task<IActionResult> CancelBooking(string id)
        {
            try
            {
                var passengerId = await GetCurrentPassengerIdAsync();
                if (passengerId == null)
                {
                    TempData["ErrorMessage"] = "Must LogIn Frist";
                    return RedirectToPage("/Account/Login");
                }

                var success = await _bookingService.CancelBookingAsync(id, passengerId.ToString()!);

                if (success)
                {
                    TempData["SuccessMessage"] = "Booking Cancled Successfuly";
                }
                else
                {
                    TempData["ErrorMessage"] = "Booking Cant Not be cancled";
                }

                return RedirectToAction("BookingDetails", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something Go Wrong";
                return RedirectToAction("BookingDetails", new { id = id });
            }
        }
        #endregion


        #region Booking Confirmation
        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(string id)
        {
            try
            {
                var success = await _bookingService.ConfirmBookingAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "Booking Confirmmed Successfuly";
                }
                else
                {
                    TempData["ErrorMessage"] = "Booking Cant Not be Confirmmed ";
                }

                return RedirectToAction("BookingDetails", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something Go Wrong";
                return RedirectToAction("BookingDetails", new { id = id });
            }
        }

        #endregion


        private bool CanCancelBooking(Booking booking)
        {
            return  booking.Status != BookingStatus.Cancelled;
        }


        private string GetBookingStatusText(BookingStatus status)
        {
            return status switch
            {
                BookingStatus.Pending => " Waiting",
                BookingStatus.Confirmed => "Confirmed",
                BookingStatus.Cancelled => "Canceled",
                _ => "UnKnowen "
            };
        }


        #region Payment Status and Method (Commented Out)
        //private string GetPaymentStatus(ICollection<Payment>? payments)
        //{
        //    if (payments == null || !payments.Any())
        //        return "Waiting";

        //    var latestPayment = payments.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
        //    return GetPaymentStatusText(latestPayment?.Status ?? PaymentStatus.Pending);
        //}
        //        private string GetPaymentStatusText(PaymentStatus status)
        //{
        //    return status switch
        //    {
        //        PaymentStatus.Pending => " Waiting",
        //        PaymentStatus.Success => "Success",
        //        PaymentStatus.Failed => "Failed",
        //        _ => " UnKnowen"
        //    };
        //}

        //private string GetPaymentMethodText(PaymentMethod method)
        //{
        //    return method switch
        //    {
        //        PaymentMethod.Cash => "نقداً",
        //        PaymentMethod.Card => "بطاقة ائتمان",
        //        PaymentMethod.Wallet => "محفظة إلكترونية",
        //        _ => "UnKnowen "
        //    };
        //}

        #endregion


        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            var model = new UserProfileViewModel
            {
                Name = User.Identity?.Name ?? "UserName",
                Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "UserName@gmail.com",
                DarkMode = false // This will be handled by JavaScript/localStorage for now
            };

            return View(model);
        }

        #region Edit Profile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found";
                    return RedirectToAction("Profile");
                }

                var model = new EditProfileViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName ?? string.Empty
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit profile page");
                TempData["ErrorMessage"] = "An error occurred while loading the edit profile page";
                return RedirectToAction("Profile");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found";
                    return RedirectToAction("Profile");
                }

                // Check if email is being changed and if it's already taken
                if (user.Email != model.Email)
                {
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null && existingUser.Id != user.Id)
                    {
                        ModelState.AddModelError("Email", "This email is already taken");
                        return View(model);
                    }
                }

                // Update user properties
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.Email; // Use email as username
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("Profile");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                TempData["ErrorMessage"] = "An error occurred while updating your profile";
                return View(model);
            }
        }
        #endregion

        #region History
        [HttpGet]
        public async Task<IActionResult> History()
        {
            try
            {
                var passengerId = await GetCurrentPassengerIdAsync();
                if (passengerId == null)
                {
                    TempData["ErrorMessage"] = "Must LogIn First";
                    return RedirectToPage("/Account/Login");
                }

                // Get all bookings for the passenger (including completed/cancelled ones)
                var allBookings = await _bookingService.GetPassengerBookingsAsync(passengerId.ToString()!);

                // Get trips that the passenger has booked (for trip history)
                var tripIds = allBookings.Select(b => b.TripId).Distinct().ToList();
                var trips = new List<Trip>();
                
                foreach (var tripId in tripIds)
                {
                    var trip = await _tripService.GetTripDetailsAsync(tripId);
                    if (trip != null)
                    {
                        trips.Add(trip);
                    }
                }

                var model = new HistoryViewModel
                {
                    Bookings = allBookings.Select(booking => new BookingHistoryViewModel
                    {
                        BookingId = booking.Id,
                        TripId = booking.TripId,
                        BookingDate = booking.BookingDate,
                        Status = booking.Status,
                        StatusText = GetBookingStatusText(booking.Status),
                        Trip = new TripSummaryViewModel
                        {
                            DepartureTime = booking.Trip?.DepartureTime ?? DateTime.MinValue,
                            OriginStation = booking.Trip?.Route?.StartStation?.Name,
                            DestinationStation = booking.Trip?.Route?.EndStation?.Name,
                            Price = booking.Trip?.Route?.Price ?? 0,
                            DriverName = $"{booking.Trip?.Driver?.appUsers?.FirstName} {booking.Trip?.Driver?.appUsers?.LastName}",
                            CarNumber = booking.Trip?.Driver?.CarNumber
                        }
                    }).OrderByDescending(b => b.BookingDate).ToList(),

                    Trips = trips.Select(trip => new TripHistoryViewModel
                    {
                        TripId = trip.Id,
                        DepartureTime = trip.DepartureTime,
                        OriginStation = trip.Route?.StartStation?.Name,
                        DestinationStation = trip.Route?.EndStation?.Name,
                        Price = trip.Route?.Price ?? 0,
                        DriverName = $"{trip.Driver?.appUsers?.FirstName} {trip.Driver?.appUsers?.LastName}",
                        CarNumber = trip.Driver?.CarNumber,
                        TotalSeats = trip.AvailableSeats,
                        BookedSeats = trip.Bookings?.Count(b => b.Status != BookingStatus.Cancelled) ?? 0,
                        PassengerBookingCount = trip.Bookings?.Count(b => b.PassengerId == passengerId && b.Status != BookingStatus.Cancelled) ?? 0
                    }).OrderByDescending(t => t.DepartureTime).ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading passenger history");
                TempData["ErrorMessage"] = "Something went wrong while loading your history";
                return View(new HistoryViewModel());
            }
        }
        #endregion
    }
}
