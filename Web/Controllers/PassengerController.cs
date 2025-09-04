using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Services;
using System.Security.Claims;
using El7aq.Domain.Entities;
using El7aq.Domain.Enums;
using Web.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Web.Models.Booking;
using Web.Models.Trip;

namespace Web.Controllers
{

    [Authorize]
    public class PassengerController : Controller
    {

        private readonly IBookingService _bookingService;
        private readonly ITripService _tripService;
        private readonly IPassengerHelperService _passengerHelperService;

        public PassengerController(
            IBookingService bookingService,
            ITripService tripService,
            IPassengerHelperService passengerHelperService)
        {
            _bookingService = bookingService;
            _tripService = tripService;
            _passengerHelperService = passengerHelperService;
        }

        // Helper method to get current passenger ID
        private async Task<int?> GetCurrentPassengerIdAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _passengerHelperService.GetPassengerIdFromUserIdAsync(userId);
        }


        #region Search for trips
        [HttpGet]
        public IActionResult SearchTrips()
        {
            var model = new SearchTripsViewModel();
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> SearchTrips(SearchTripsViewModel model)
        { 
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var trips = await _tripService.SearchTripsAsync(
                    model.OriginCityId,
                    model.DestinationCityId,
                    model.Date);
                model.SearchResults = trips.Select(trip => new TripResultViewModel
                {
                    TripId = int.Parse(trip.Id),
                    DepartureTime = trip.DepartureTime,
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

                ModelState.AddModelError("", "Something going wrong");
                return View(model);
            }

        }
        #endregion


        #region Trip Details

        [HttpGet]
        public async Task<IActionResult> TripDetails(int id)
        { 
        
            var trip = await _tripService.GetTripDetailsAsync(id.ToString());
            if (trip == null)
            {
                TempData["ErrorMessage"] = "Trip Not Found";
                return RedirectToAction("SearchTrips");
            }

            var bookedSeats = trip.Bookings?.Count(b => b.Status != BookingStatus.Cancelled) ?? 0;
            var model = new TripDetailsViewModel
            {
                TripId = int.Parse(trip.Id),
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
                if (!passengerId.HasValue)
                {
                    TempData["ErrorMessage"] = "Must LogIn Frist";
                    return RedirectToAction("Login", "Account");
                }

                var booking = await _bookingService.CreateBookingAsync(passengerId.ToString()!, tripId);

                TempData["SuccessMessage"] = "The seat has been successfully booked.";
                return RedirectToAction("BookingDetails", new { id = booking.Id });

            }
            catch (Exception ex)
            {
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
                if (!passengerId.HasValue)
                {
                    TempData["ErrorMessage"] = "Must LogIn Frist";
                    return RedirectToAction("Login", "Account");
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
                            DepartureTime = booking.Trip?.DepartureTime ?? DateTime.MinValue,
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
                if (!passengerId.HasValue)
                {
                    TempData["ErrorMessage"] = "Must LogIn Frist";
                    return RedirectToAction("Login", "Account");
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
                        TimeUntilDeparture = booking.Trip?.DepartureTime - DateTime.UtcNow
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
                if (!passengerId.HasValue || booking.PassengerId != passengerId.Value.ToString())
                {
                    TempData["ErrorMessage"] = "You are Forbiden";
                    return RedirectToAction("MyBookings");
                }

                var model = new BookingDetailsViewModel
                {
                    BookingId = int.Parse(booking.Id),
                    TripId = int.Parse(booking.TripId),
                    BookingDate = booking.BookingDate,
                    Status = booking.Status,
                    StatusText = GetBookingStatusText(booking.Status),
                    Trip = new TripDetailsViewModel
                    {
                        TripId = int.Parse(booking.Trip.Id),
                        DepartureTime = booking.Trip?.DepartureTime ?? DateTime.MinValue,
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
                if (!passengerId.HasValue)
                {
                    TempData["ErrorMessage"] = "Must LogIn Frist";
                    return RedirectToAction("Login", "Account");
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
            return booking.Trip?.DepartureTime > DateTime.UtcNow.AddHours(2) &&
                   booking.Status != BookingStatus.Cancelled;
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
    }
}
