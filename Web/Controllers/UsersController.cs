using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;
namespace Web.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<AppUsers> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<AppUsers> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var usersList = await _userManager.Users.ToListAsync();
            var users = new List<UserViewModel>();

            foreach (var user in usersList)
            {
                var roles = await _userManager.GetRolesAsync(user);

                users.Add(new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return View(users);
        }

        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var roles = await _roleManager.Roles.ToListAsync();
            var viewModel = new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = new List<RoleViewModel>()
            };

            foreach (var role in roles)
            {
                bool isSelected = await _userManager.IsInRoleAsync(user, role.Name);
                viewModel.Roles.Add(new RoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = isSelected
                });
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(UserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in model.Roles)
            {
                if (userRoles.Contains(role.RoleName) && !role.IsSelected)
                    await _userManager.RemoveFromRoleAsync(user, role.RoleName);

                if (!userRoles.Contains(role.RoleName) && role.IsSelected)
                    await _userManager.AddToRoleAsync(user, role.RoleName);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
