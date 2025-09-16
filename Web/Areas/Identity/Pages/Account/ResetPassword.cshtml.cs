// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Core.Entities;
using Infrastructure.Services;

namespace Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<AppUsers> _userManager;
        private readonly ICodeVerificationService _codeVerificationService;

        public ResetPasswordModel(UserManager<AppUsers> userManager, ICodeVerificationService codeVerificationService)
        {
            _userManager = userManager;
            _codeVerificationService = codeVerificationService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            public string Code { get; set; }
        }

        public IActionResult OnGet()
        {
            // Check if user has verified their code
            if (!TempData.ContainsKey("ResetEmail") || !TempData.ContainsKey("CodeVerified"))
            {
                return RedirectToPage("./ForgotPassword");
            }

            // Get the email from TempData
            var email = TempData["ResetEmail"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("./ForgotPassword");
            }

            // Store email back in TempData for the POST method
            TempData["ResetEmail"] = email;

            Input = new InputModel
            {
                Email = email
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Verify that user has completed code verification
            if (!TempData.ContainsKey("ResetEmail") || !TempData.ContainsKey("CodeVerified"))
            {
                return RedirectToPage("./ForgotPassword");
            }

            var email = TempData["ResetEmail"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("./ForgotPassword");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            // Generate a new password reset token (since we're not using the old token system)
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, Input.Password);
            
            if (result.Succeeded)
            {
                // Invalidate the verification code after successful password reset
                await _codeVerificationService.InvalidateCodeAsync(email);
                
                // Clear TempData
                TempData.Remove("ResetEmail");
                TempData.Remove("CodeVerified");
                
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}
