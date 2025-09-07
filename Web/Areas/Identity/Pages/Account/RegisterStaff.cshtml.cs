using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core.Entities;
using Infrastructure;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Identity.Pages.Account
{
    public class RegisterStaffModel : PageModel
    {
        private readonly SignInManager<AppUsers> _signInManager;
        private readonly UserManager<AppUsers> _userManager;
        private readonly ILogger<RegisterStaffModel> _logger;
        private readonly ApplicationDbContext _db;

        public RegisterStaffModel(
            UserManager<AppUsers> userManager,
            SignInManager<AppUsers> signInManager,
            ILogger<RegisterStaffModel> logger,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public List<Station> Stations { get; set; } = new List<Station>();

        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Station")]
            public string StationId { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            Stations = await _db.Stations.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new AppUsers
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Staff created a new account with password.");

                    // Add user to Staff role
                    await _userManager.AddToRoleAsync(user, "Staff");

                    // Add role claim
                    var roleClaim = new Claim(ClaimTypes.Role, "Staff");
                    await _userManager.AddClaimAsync(user, roleClaim);

                    // Create StaffProfile with proper ID generation
                    var staffProfile = new StaffProfile
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        StationId = Input.StationId
                    };

                    _db.Staffs.Add(staffProfile);
                    await _db.SaveChangesAsync();

                    TempData["Success"] = "Staff account created successfully.";
                    return new RedirectToActionResult("ManagerHome", "Manager", null);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Reload stations for the view
            Stations = await _db.Stations.ToListAsync();
            return Page();
        }
    }
}
