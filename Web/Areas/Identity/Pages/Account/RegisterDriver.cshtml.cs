using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core.Entities;
using Infrastructure;
using System.Security.Claims;

namespace Web.Areas.Identity.Pages.Account
{
    public class RegisterDriverModel : PageModel
    {
        private readonly SignInManager<AppUsers> _signInManager;
        private readonly UserManager<AppUsers> _userManager;
        private readonly ILogger<RegisterDriverModel> _logger;
        private readonly ApplicationDbContext _db;

        public RegisterDriverModel(
            UserManager<AppUsers> userManager,
            SignInManager<AppUsers> signInManager,
            ILogger<RegisterDriverModel> logger,
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
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
            [Display(Name = "License Number")]
            public string LicenseNumber { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
            [Display(Name = "Car Number")]
            public string CarNumber { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
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
                    _logger.LogInformation("Driver created a new account with password.");

                    // Add user to Driver role
                    await _userManager.AddToRoleAsync(user, "Driver");

                    // Add role claim
                    var roleClaim = new Claim(ClaimTypes.Role, "Driver");
                    await _userManager.AddClaimAsync(user, roleClaim);

                    // Create DriverProfile with proper ID generation
                    var driverProfile = new DriverProfile
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        LicenseNumber = Input.LicenseNumber,
                        CarNumber = Input.CarNumber
                    };

                    _db.Drivers.Add(driverProfile);
                    await _db.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Account", "Driver");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
