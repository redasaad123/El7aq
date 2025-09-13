using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Web.Models;
using Web.Models.Driver;
using Infrastructure.Services;

namespace Web.Controllers
{
    [Authorize(Roles = "Driver,Staff")]
    public class DriverController : Controller
    {
        private readonly UserManager<AppUsers> userManager;
        private readonly IUnitOfWork<DriverOrder> driverOrderUnitOfWork;
        private readonly IUnitOfWork<DriverProfile> drivernUitOfWork;
        private readonly IUnitOfWork<Trip> tripUnitOfWork;
        private readonly IUnitOfWork<TripDriverQueue> tripQueueUnitOfWork;
        private readonly IUnitOfWork<Station> stationUnitOfWork;
        private readonly INotificationService notificationService;

        public DriverController(
            UserManager<AppUsers> userManager,
            IUnitOfWork<DriverOrder> DriverOrderUnitOfWork, 
            IUnitOfWork<DriverProfile> DrivernUitOfWork,
            IUnitOfWork<Trip> tripUnitOfWork,
            IUnitOfWork<TripDriverQueue> tripQueueUnitOfWork,
            IUnitOfWork<Station> stationUnitOfWork,
            INotificationService notificationService)
        {
            this.userManager = userManager;
            driverOrderUnitOfWork = DriverOrderUnitOfWork;
            drivernUitOfWork = DrivernUitOfWork;
            this.tripUnitOfWork = tripUnitOfWork;
            this.tripQueueUnitOfWork = tripQueueUnitOfWork;
            this.stationUnitOfWork = stationUnitOfWork;
            this.notificationService = notificationService;
        }

        public async Task<IActionResult> Home()
        {
            var user = await userManager.GetUserAsync(User);
            
            // Use comprehensive fake data for demonstration
            var model = new DriverHomeViewModel
            {
                DriverId = "fake-driver-001",
                DriverName = $"{user.FirstName} {user.LastName}",
                StationName = "Downtown Station",
                StationCity = "Cairo",
                QueuePosition = 2,
                IsMyTurn = false,
                CanStartTrip = false,
                TripQueueId = "fake-queue-001"
            };

            // Simulate different scenarios based on time with more realistic data
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            
            if (hour >= 6 && hour < 10)
            {
                // Morning rush (6 AM - 10 AM) - driver is ready
                model.QueuePosition = 1;
                model.IsMyTurn = true;
                model.CanStartTrip = true;
                model.StationName = "Downtown Station";
                
                model.CurrentTrip = new TripDetailsViewModel
                {
                    TripId = "trip-morning-001",
                    RouteId = "route-airport-001",
                    StartStation = "Downtown Station",
                    EndStation = "Cairo Airport Terminal 3",
                    StartCity = "Cairo",
                    EndCity = "Cairo",
                    DepartureTime = DateTime.Now.AddMinutes(30),
                    AvailableSeats = 15,
                    BookedSeats = 12,
                    Price = 35.00m,
                    Status = DriverStatus.Queued
                };
            }
            else if (hour >= 10 && hour < 14)
            {
                // Midday (10 AM - 2 PM) - waiting in queue
                model.QueuePosition = 2;
                model.IsMyTurn = false;
                model.CanStartTrip = false;
                model.StationName = "City Center Station";
                
                model.CurrentTrip = new TripDetailsViewModel
                {
                    TripId = "trip-midday-001",
                    RouteId = "route-mall-001",
                    StartStation = "City Center Station",
                    EndStation = "Mall of Egypt",
                    StartCity = "Cairo",
                    EndCity = "Giza",
                    DepartureTime = DateTime.Now.AddHours(1).AddMinutes(15),
                    AvailableSeats = 12,
                    BookedSeats = 7,
                    Price = 20.00m,
                    Status = DriverStatus.Queued
                };
            }
            else if (hour >= 14 && hour < 18)
            {
                // Afternoon (2 PM - 6 PM) - driver is ready
                model.QueuePosition = 1;
                model.IsMyTurn = true;
                model.CanStartTrip = true;
                model.StationName = "Heliopolis Station";
                
                model.CurrentTrip = new TripDetailsViewModel
                {
                    TripId = "trip-afternoon-001",
                    RouteId = "route-newcairo-001",
                    StartStation = "Heliopolis Station",
                    EndStation = "New Cairo City",
                    StartCity = "Cairo",
                    EndCity = "Cairo",
                    DepartureTime = DateTime.Now.AddMinutes(45),
                    AvailableSeats = 18,
                    BookedSeats = 15,
                    Price = 28.50m,
                    Status = DriverStatus.Queued
                };
            }
            else if (hour >= 18 && hour < 22)
            {
                // Evening rush (6 PM - 10 PM) - waiting in queue
                model.QueuePosition = 3;
                model.IsMyTurn = false;
                model.CanStartTrip = false;
                model.StationName = "Nasr City Station";
                
                model.CurrentTrip = new TripDetailsViewModel
                {
                    TripId = "trip-evening-001",
                    RouteId = "route-downtown-001",
                    StartStation = "Nasr City Station",
                    EndStation = "Downtown Cairo",
                    StartCity = "Cairo",
                    EndCity = "Cairo",
                    DepartureTime = DateTime.Now.AddHours(2),
                    AvailableSeats = 14,
                    BookedSeats = 11,
                    Price = 22.00m,
                    Status = DriverStatus.Queued
                };
            }
            else
            {
                // Night time (10 PM - 6 AM) - no active queue
                model.QueuePosition = 0;
                model.IsMyTurn = false;
                model.CanStartTrip = false;
                model.StationName = "Main Station";
                model.CurrentTrip = null;
            }

            return View(model);
        }

        public async Task<IActionResult> Order()
        {
            var user = await userManager.GetUserAsync(User);
            
            // Use fake data for demonstration
            var fakeDrivers = new List<DriverQueueItemViewModel>
            {
                new DriverQueueItemViewModel
                {
                    Position = 1,
                    DriverName = "Ahmed Hassan",
                    IsCurrentDriver = false,
                    Status = "Ready"
                },
                new DriverQueueItemViewModel
                {
                    Position = 2,
                    DriverName = $"{user.FirstName} {user.LastName}",
                    IsCurrentDriver = true,
                    Status = "Waiting"
                },
                new DriverQueueItemViewModel
                {
                    Position = 3,
                    DriverName = "Mohamed Ali",
                    IsCurrentDriver = false,
                    Status = "Waiting"
                },
                new DriverQueueItemViewModel
                {
                    Position = 4,
                    DriverName = "Omar Khaled",
                    IsCurrentDriver = false,
                    Status = "Waiting"
                }
            };

            var fakeTrips = new List<TripQueueInfoViewModel>
            {
                new TripQueueInfoViewModel
                {
                    TripId = "trip-1",
                    Route = "Downtown Station → Airport Terminal",
                    DepartureTime = DateTime.Now.AddHours(1),
                    QueueCount = 4
                },
                new TripQueueInfoViewModel
                {
                    TripId = "trip-2",
                    Route = "City Center → Mall of Egypt",
                    DepartureTime = DateTime.Now.AddHours(3),
                    QueueCount = 2
                },
                new TripQueueInfoViewModel
                {
                    TripId = "trip-3",
                    Route = "Heliopolis → New Cairo",
                    DepartureTime = DateTime.Now.AddHours(5),
                    QueueCount = 1
                }
            };

            // Simulate different scenarios based on time
            var hour = DateTime.Now.Hour;
            var currentPosition = 2;
            var isMyTurn = false;

            if (hour >= 8 && hour < 10)
            {
                // Morning rush - driver is ready
                currentPosition = 1;
                isMyTurn = true;
                fakeDrivers[0].IsCurrentDriver = true;
                fakeDrivers[0].Status = "Ready";
                fakeDrivers[1].IsCurrentDriver = false;
                fakeDrivers[1].Position = 1;
                fakeDrivers[1].Status = "Ready";
            }
            else if (hour >= 16 && hour < 18)
            {
                // Evening rush - driver is ready
                currentPosition = 1;
                isMyTurn = true;
                fakeDrivers[0].IsCurrentDriver = true;
                fakeDrivers[0].Status = "Ready";
                fakeDrivers[1].IsCurrentDriver = false;
                fakeDrivers[1].Position = 1;
                fakeDrivers[1].Status = "Ready";
            }

            var model = new DriverQueueViewModel
            {
                DriverId = "fake-driver-id",
                DriverName = $"{user.FirstName} {user.LastName}",
                CurrentPosition = currentPosition,
                TotalDrivers = fakeDrivers.Count,
                IsMyTurn = isMyTurn,
                DriversInQueue = fakeDrivers,
                TodayTrips = fakeTrips
            };

            return View(model);
        }

        public async Task<IActionResult> Account()
        {
            var user = await userManager.GetUserAsync(User);
            var driverProfile = drivernUitOfWork.Entity.Find(d=> d.UserId == user.Id);

            if (driverProfile == null)
            {
                TempData["Error"] = "Driver profile not found. Please contact your manager to set up your driver profile.";
                return View(new DriverProfileViewModel
                {
                    Name = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    Phone = user.PhoneNumber ?? ""
                });
            }



            var driver = new DriverProfileViewModel
            {
                LicenseNumber = driverProfile.LicenseNumber,
                CarNumber = driverProfile.CarNumber,
                Name = user.FirstName + " " + user.LastName,
                Email = user.Email,
                Phone = user.PhoneNumber,
            };

            return View(driver);

        }


        public async Task<IActionResult> UpdateDriver()
        {

            var user = await userManager.GetUserAsync(User);
            var driverProfile = drivernUitOfWork.Entity.Find(d => d.UserId == user.Id);


            var driver = new DriverProfileViewModel
            {
                LicenseNumber = driverProfile.LicenseNumber,
                CarNumber = driverProfile.CarNumber,
                Name = user.FirstName + " " + user.LastName,
                Email = user.Email,
                Phone = user.PhoneNumber,
            };

            return View(driver);
        }


        [HttpPost]

        public async Task<IActionResult> UpdateDriver(DriverProfileViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                var driverProfile = drivernUitOfWork.Entity.Find(d => d.UserId == user.Id);
                if (driverProfile == null)
                {
                    TempData["Error"] = "Driver profile not found. Please contact your manager to set up your driver profile.";
                    return View("Account", model);
                }
                driverProfile.CarNumber = model.CarNumber;
                driverProfile.LicenseNumber = model.LicenseNumber;
                user.PhoneNumber = model.Phone;
                
                // Safely split the name
                var nameParts = model.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (nameParts.Length >= 2)
                {
                    user.FirstName = nameParts[0];
                    user.LastName = string.Join(" ", nameParts.Skip(1));
                }
                else if (nameParts.Length == 1)
                {
                    user.FirstName = nameParts[0];
                    user.LastName = "";
                }
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    drivernUitOfWork.Entity.Update(driverProfile);
                    await drivernUitOfWork.SaveAsync();
                    return RedirectToAction("Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View("Account", model);

        }

        [HttpPost]
        public async Task<IActionResult> StartTrip(string tripQueueId)
        {
            if (string.IsNullOrEmpty(tripQueueId))
            {
                TempData["Error"] = "Invalid trip queue ID.";
                return RedirectToAction("Home");
            }

            var user = await userManager.GetUserAsync(User);
            
            // Simulate trip start with comprehensive fake data
            var hour = DateTime.Now.Hour;
            var successMessages = new Dictionary<int, string>
            {
                { 6, $"Trip started successfully! You are now driving from Downtown Station to Cairo Airport Terminal 3. Departure time: {DateTime.Now.AddMinutes(30):HH:mm}" },
                { 7, $"Trip started successfully! You are now driving from Downtown Station to Cairo Airport Terminal 3. Departure time: {DateTime.Now.AddMinutes(30):HH:mm}" },
                { 8, $"Trip started successfully! You are now driving from Downtown Station to Cairo Airport Terminal 3. Departure time: {DateTime.Now.AddMinutes(30):HH:mm}" },
                { 9, $"Trip started successfully! You are now driving from Downtown Station to Cairo Airport Terminal 3. Departure time: {DateTime.Now.AddMinutes(30):HH:mm}" },
                { 14, $"Trip started successfully! You are now driving from Heliopolis Station to New Cairo City. Departure time: {DateTime.Now.AddMinutes(45):HH:mm}" },
                { 15, $"Trip started successfully! You are now driving from Heliopolis Station to New Cairo City. Departure time: {DateTime.Now.AddMinutes(45):HH:mm}" },
                { 16, $"Trip started successfully! You are now driving from Heliopolis Station to New Cairo City. Departure time: {DateTime.Now.AddMinutes(45):HH:mm}" },
                { 17, $"Trip started successfully! You are now driving from Heliopolis Station to New Cairo City. Departure time: {DateTime.Now.AddMinutes(45):HH:mm}" }
            };

            if (successMessages.ContainsKey(hour))
            {
                // During rush hours when it's the driver's turn, allow trip start
                TempData["Success"] = successMessages[hour];
            }
            else
            {
                // Outside rush hours or not driver's turn, show error
                var errorMessages = new Dictionary<int, string>
                {
                    { 10, "It's not your turn to start the trip. You are #2 in the queue for City Center → Mall of Egypt." },
                    { 11, "It's not your turn to start the trip. You are #2 in the queue for City Center → Mall of Egypt." },
                    { 12, "It's not your turn to start the trip. You are #2 in the queue for City Center → Mall of Egypt." },
                    { 13, "It's not your turn to start the trip. You are #2 in the queue for City Center → Mall of Egypt." },
                    { 18, "It's not your turn to start the trip. You are #3 in the queue for Nasr City → Downtown Cairo." },
                    { 19, "It's not your turn to start the trip. You are #3 in the queue for Nasr City → Downtown Cairo." },
                    { 20, "It's not your turn to start the trip. You are #3 in the queue for Nasr City → Downtown Cairo." },
                    { 21, "It's not your turn to start the trip. You are #3 in the queue for Nasr City → Downtown Cairo." }
                };

                TempData["Error"] = errorMessages.ContainsKey(hour) ? errorMessages[hour] : "No active trips available at this time. Please check back during business hours.";
            }

            return RedirectToAction("Home");
        }
    }
}
