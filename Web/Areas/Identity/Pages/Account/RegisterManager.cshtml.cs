using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core.Entities;
using Infrastructure;
using System.Security.Claims;

namespace Web.Areas.Identity.Pages.Account
{
    public class RegisterManagerModel : PageModel
    {
        private readonly SignInManager<AppUsers> _signInManager;
        private readonly UserManager<AppUsers> _userManager;
        private readonly ILogger<RegisterManagerModel> _logger;
        private readonly ApplicationDbContext _db;

        public RegisterManagerModel(
            UserManager<AppUsers> userManager,
            SignInManager<AppUsers> signInManager,
            ILogger<RegisterManagerModel> logger,
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
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [Display(Name = "Department")]
            public string Department { get; set; }

            [StringLength(500, ErrorMessage = "The {0} must be at max {1} characters long.")]
            [Display(Name = "Notes (Optional)")]
            public string? Notes { get; set; }
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

                // Debug logging
                _logger.LogInformation($"Manager registration attempt for email: {Input.Email}");
                _logger.LogInformation($"User creation result: Succeeded={result.Succeeded}");
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError($"User creation error: {error.Description}");
                    }
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation("Manager created a new account with password.");

                    // Add Manager role claim
                    var roleClaim = new Claim(ClaimTypes.Role, "Manager");
                    await _userManager.AddClaimAsync(user, roleClaim);

                    // Add FirstName and LastName claims
                    var firstNameClaim = new Claim("FirstName", Input.FirstName);
                    var lastNameClaim = new Claim("LastName", Input.LastName);
                    await _userManager.AddClaimAsync(user, firstNameClaim);
                    await _userManager.AddClaimAsync(user, lastNameClaim);

                    // Add user to Manager role
                    await _userManager.AddToRoleAsync(user, "Manager");

                    // Create ManagerProfile with proper ID generation
                    var managerProfile = new ManagerProfile
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        Department = Input.Department,
                        Notes = Input.Notes
                    };

                    _db.Managers.Add(managerProfile);
                    await _db.SaveChangesAsync();

                    // Debug: Try to find the user after creation
                    var createdUser = await _userManager.FindByEmailAsync(Input.Email);
                    _logger.LogInformation($"User found after creation: {createdUser != null}, UserName: {createdUser?.UserName}");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("ManagerHome", "Manager");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.LogError($"Registration error: {error.Description}");
                }
            }

            return Page();
        }
    }
}
