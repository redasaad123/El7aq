using Microsoft.AspNetCore.Mvc;
using Core.Enums;

namespace Infrastructure.Services
{
    /// <summary>
    /// Service responsible for handling role-based routing logic.
    /// Uses the Strategy pattern to eliminate deeply nested if statements.
    /// </summary>
    public class RoleRoutingService : IRoleRoutingService
    {
        private readonly Dictionary<string, Func<IActionResult>> _roleRedirects;

        public RoleRoutingService()
        {
            _roleRedirects = new Dictionary<string, Func<IActionResult>>
            {
                { UserRole.Manager.ToString(), () => new RedirectToActionResult("ManagerHome", "Manager", null) },
                { UserRole.Driver.ToString(), () => new RedirectToActionResult("Home", "Driver", null) },
                { UserRole.Staff.ToString(), () => new RedirectToActionResult("Home", "Staff", null) }
                // Note: Passenger role is intentionally omitted as they should stay on the home page
            };
        }

        /// <summary>
        /// Gets the appropriate redirect action for the given user role.
        /// Returns null if no redirect is needed (e.g., for passengers).
        /// </summary>
        /// <param name="userRole">The user's role as a string</param>
        /// <returns>Redirect action result or null if no redirect needed</returns>
        public IActionResult? GetRedirectForRole(string userRole)
        {
            if (string.IsNullOrEmpty(userRole))
                return null;

            return _roleRedirects.TryGetValue(userRole, out var redirectAction) 
                ? redirectAction() 
                : null;
        }
    }
}
