using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using System.Security.Claims;

namespace Web.Controllers
{
    public class FixClaimsController : Controller
    {
        private readonly UserManager<AppUsers> _userManager;

        public FixClaimsController(UserManager<AppUsers> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> FixUserClaims(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Content("Please provide an email parameter: /FixClaims/FixUserClaims?email=test@example.com");
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Json(new { Error = "User not found" });
                }

                // Check if claims already exist
                var existingClaims = await _userManager.GetClaimsAsync(user);
                var hasFirstName = existingClaims.Any(c => c.Type == "FirstName");
                var hasLastName = existingClaims.Any(c => c.Type == "LastName");

                if (hasFirstName && hasLastName)
                {
                    return Json(new { Message = "Claims already exist", FirstName = user.FirstName, LastName = user.LastName });
                }

                // Add missing claims
                if (!hasFirstName)
                {
                    var firstNameClaim = new Claim("FirstName", user.FirstName);
                    await _userManager.AddClaimAsync(user, firstNameClaim);
                }

                if (!hasLastName)
                {
                    var lastNameClaim = new Claim("LastName", user.LastName);
                    await _userManager.AddClaimAsync(user, lastNameClaim);
                }

                return Json(new { 
                    Message = "Claims added successfully", 
                    FirstName = user.FirstName, 
                    LastName = user.LastName,
                    AddedFirstName = !hasFirstName,
                    AddedLastName = !hasLastName
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }
    }
}
