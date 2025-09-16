using System.Diagnostics;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using System.Security.Claims;
using Infrastructure.Services;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork<Station> _stationUow;
        private readonly IRoleRoutingService _roleRoutingService;

        public HomeController(IUnitOfWork<Station> stationUow, ILogger<HomeController> logger, IRoleRoutingService roleRoutingService)
        {
            _stationUow = stationUow;
            _logger = logger;
            _roleRoutingService = roleRoutingService;
        }

        public async Task<IActionResult> Index()
        {
            // Check if user is signed in and redirect based on role
            if (User.Identity.IsAuthenticated)
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var redirectResult = _roleRoutingService.GetRedirectForRole(userRole);
                
                if (redirectResult != null)
                {
                    return redirectResult;
                }
                // Removed automatic redirect for passengers - let them stay on home page
            }

            try
            {
                
                var stations = await _stationUow.Entity.GetAllAsyncAsQuery().ToListAsync();

               
                ViewBag.Stations = stations.Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = $"{s.Name} - {s.City}"
                }).ToList();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stations for home page");

              
                ViewBag.Stations = new List<SelectListItem>();
                return View();
            }
        }

        [HttpPost]
        public IActionResult SearchTrips(string OriginStationId, string DestinationStationId)
        {
            
            if (string.IsNullOrEmpty(OriginStationId) || string.IsNullOrEmpty(DestinationStationId))
            {
                TempData["ErrorMessage"] = "Please select the departure station and the destination.";
                return RedirectToAction("Index");
            }

            if (OriginStationId == DestinationStationId)
            {
                TempData["ErrorMessage"] = "The starting station and the destination cannot be the same.";
                return RedirectToAction("Index");
            }

       
            return RedirectToAction("SearchTrips", "Passenger", new
            {
                OriginStationId = OriginStationId,
                DestinationStationId = DestinationStationId
            });
        }



        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
