using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core.Entities;
using Infrastructure.Services;

namespace Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class VerifyCodeModel : PageModel
    {
        private readonly ICodeVerificationService _codeVerificationService;

        public VerifyCodeModel(ICodeVerificationService codeVerificationService)
        {
            _codeVerificationService = codeVerificationService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Verification code is required")]
            [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be exactly 6 digits")]
            [RegularExpression(@"^\d{6}$", ErrorMessage = "Code must contain only numbers")]
            public string Code { get; set; }
        }

        public IActionResult OnGet()
        {
            // Check if there's an email in TempData from the forgot password step
            if (!TempData.ContainsKey("ResetEmail"))
            {
                return RedirectToPage("./ForgotPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get the email from TempData
            if (!TempData.TryGetValue("ResetEmail", out var emailObj) || emailObj == null)
            {
                return RedirectToPage("./ForgotPassword");
            }

            var email = emailObj.ToString();

            // Validate the verification code
            var isValidCode = await _codeVerificationService.ValidateVerificationCodeAsync(email, Input.Code);
            
            if (!isValidCode)
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired verification code. Please try again.");
                return Page();
            }

            // Code is valid, store email in TempData for the reset password step
            TempData["ResetEmail"] = email;
            TempData["CodeVerified"] = true;

            return RedirectToPage("./ResetPassword");
        }
    }
}
