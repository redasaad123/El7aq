using Microsoft.AspNetCore.Identity;
using Core.Entities;
using Core.Enums;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class RoleSeederService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUsers> _userManager;
        private readonly ILogger<RoleSeederService> _logger;

        public RoleSeederService(
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUsers> userManager,
            ILogger<RoleSeederService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task SeedRolesAsync()
        {
            var roles = new[] { "Manager", "Driver", "Staff", "Passenger" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation($"Created role: {role}");
                }
            }
        }

        public async Task SeedStaffUserAsync()
        {
            const string staffEmail = "staff@el7aq.com";
            const string staffPassword = "Staff@123456";
            const string staffRole = "Staff";

            var staffUser = await _userManager.FindByEmailAsync(staffEmail);
            if (staffUser == null)
            {
                staffUser = new AppUsers
                {
                    UserName = staffEmail,
                    Email = staffEmail,
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Staff"
                };

                var result = await _userManager.CreateAsync(staffUser, staffPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(staffUser, staffRole);
                    _logger.LogInformation($"Staff user created: {staffEmail}");
                }
                else
                {
                    _logger.LogError($"Failed to create staff user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
