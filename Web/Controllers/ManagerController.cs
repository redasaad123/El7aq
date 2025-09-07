using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly ILogger<ManagerController> _logger;

        public ManagerController(ILogger<ManagerController> logger)
        {
            _logger = logger;
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
    }
}
