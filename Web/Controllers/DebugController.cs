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

        public DebugController(UserManager<AppUsers> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> CheckUser(string email)
        {
            try
            {
                // Check if user exists by email
                var user = await _userManager.FindByEmailAsync(email);
                
                // Check if user exists by username
                var userByName = await _userManager.FindByNameAsync(email);
                
                // Check database directly
                var userFromDb = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
                
                // Check managers table
                var managerProfile = user != null ? await _db.Managers.FirstOrDefaultAsync(m => m.UserId == user.Id) : null;
                
                var result = new
                {
                    Email = email,
                    UserFoundByEmail = user != null,
                    UserFoundByName = userByName != null,
                    UserFromDb = userFromDb != null,
                    UserName = user?.UserName,
                    EmailFromUser = user?.Email,
                    Id = user?.Id,
                    ManagerProfile = managerProfile != null ? new { managerProfile.Department, managerProfile.Notes } : null,
                    AllUsers = await _db.Users.Select(u => new { u.Email, u.UserName, u.Id }).ToListAsync()
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        public async Task<IActionResult> TestPassword(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Json(new { Error = "User not found" });
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, password);
                
                return Json(new
                {
                    Email = email,
                    PasswordValid = passwordValid,
                    UserName = user.UserName,
                    UserId = user.Id
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }
    }
}
