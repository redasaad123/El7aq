using Microsoft.AspNetCore.Mvc;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;

namespace Web.Controllers
{
    public class LocationController : Controller
    {
        private readonly IGeolocationService _geoService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LocationController> _logger;

        public LocationController(
            IGeolocationService geoService,
            ApplicationDbContext context,
            ILogger<LocationController> logger)
        {
            _geoService = geoService;
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetLocation(LocationRequest request)
        {
            _logger.LogInformation("GetLocation called with Lat={Lat}, Long={Long}", request.Lat, request.Long);

            string? address = await _geoService.GetAddressFromCoordinates(request.Lat, request.Long);
            _logger.LogInformation("Address returned: {Address}", address ?? "null");

            return Json(new { Address = address });
        }

        [HttpPost]
        public async Task<IActionResult> GetNearestDrivers(LocationRequest request)
        {
            var drivers = await _context.Drivers
                .Where(d => d.Lat != null && d.Long != null)
                .ToListAsync();

            if (!drivers.Any())
                return Json(new { Error = "No drivers available." });

            var driverDistances = new List<object>();
            foreach (var driver in drivers)
            {
                double distance = _geoService.CalculateDistance(
                    Convert.ToDouble(driver.Lat),
                    Convert.ToDouble(driver.Long),
                    request.Lat,
                    request.Long);

                string? address = null;
                try
                {
                    address = await _geoService.GetAddressFromCoordinates(
                        Convert.ToDouble(driver.Lat),
                        Convert.ToDouble(driver.Long));

                    if (!string.IsNullOrEmpty(address))
                    {
                        var parts = address.Split(',');
                        address = parts.Length >= 3 ? parts[2].Trim() : address;
                    }
                }
                catch
                {
                    address = "العنوان غير متوفر";
                }

                driverDistances.Add(new
                {
                    CarNumber = driver.CarNumber,
                    DistanceKm = Math.Round(distance, 2),
                    Address = address
                });
            }

            var nearestDrivers = driverDistances
                .OrderBy(d => ((dynamic)d).DistanceKm)
                .Take(3)
                .ToList();

            // ترميز UTF-8 للعربية في JSON
            return Json(nearestDrivers, new System.Text.Json.JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

    }

    public class LocationRequest
    {
        public double Lat { get; set; }
        public double Long { get; set; }
    }
}
