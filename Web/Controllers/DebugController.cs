using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers
{
    public class DebugController : Controller
    {
        private readonly UserManager<AppUsers> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<DebugController> _logger;

        public DebugController(
            UserManager<AppUsers> userManager,
            ApplicationDbContext db,
            ILogger<DebugController> logger)
        {
            _userManager = userManager;
            _db = db;
            _logger = logger;
        }

        public async Task<IActionResult> TestRegistration()
        {
            try
            {
                // Test user creation
                var testUser = new AppUsers
                {
                    UserName = "test@test.com",
                    Email = "test@test.com",
                    FirstName = "Test",
                    LastName = "User"
                };

                var result = await _userManager.CreateAsync(testUser, "test123");
                
                ViewBag.UserCreationResult = result.Succeeded;
                ViewBag.Errors = result.Errors.Select(e => e.Description).ToList();
                
                if (result.Succeeded)
                {
                    // Test role assignment
                    var roleResult = await _userManager.AddToRoleAsync(testUser, "Passenger");
                    ViewBag.RoleAssignmentResult = roleResult.Succeeded;
                    ViewBag.RoleErrors = roleResult.Errors.Select(e => e.Description).ToList();
                    
                    // Test profile creation
                    var passengerProfile = new PassengerProfile
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = testUser.Id
                    };
                    
                    _db.Passengers.Add(passengerProfile);
                    var saveResult = await _db.SaveChangesAsync();
                    ViewBag.ProfileCreationResult = saveResult > 0;
                }
                
                // Check roles
                var roles = await _db.Roles.ToListAsync();
                ViewBag.Roles = roles.Select(r => r.Name).ToList();
                
                // Check users
                var users = await _userManager.Users.ToListAsync();
                ViewBag.Users = users.Select(u => new { u.Email, u.FirstName, u.LastName }).ToList();
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test registration");
                ViewBag.Error = ex.Message;
                return View();
            }
        }
    }
}