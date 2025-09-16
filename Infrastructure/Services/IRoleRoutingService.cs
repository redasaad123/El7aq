using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Services
{
    /// <summary>
    /// Interface for role-based routing service.
    /// Provides a clean abstraction for handling user role redirects.
    /// </summary>
    public interface IRoleRoutingService
    {
        /// <summary>
        /// Gets the appropriate redirect action for the given user role.
        /// </summary>
        /// <param name="userRole">The user's role as a string</param>
        /// <returns>Redirect action result or null if no redirect needed</returns>
        IActionResult? GetRedirectForRole(string userRole);
    }
}
