// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Core.Entities;
using Infrastructure.Services;

namespace Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<AppUsers> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ICodeVerificationService _codeVerificationService;
        private readonly ILogger<ForgotPasswordModel> _logger;

        public ForgotPasswordModel(UserManager<AppUsers> userManager, IEmailSender emailSender, ICodeVerificationService codeVerificationService, ILogger<ForgotPasswordModel> logger)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _codeVerificationService = codeVerificationService;
            _logger = logger;
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
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // Generate a 6-digit verification code
                var verificationCode = await _codeVerificationService.GenerateVerificationCodeAsync(Input.Email);
                _logger.LogInformation("Generated verification code for email: {Email}", Input.Email);
                
                // Store the email in TempData for the next step
                TempData["ResetEmail"] = Input.Email;
                
                // Send verification code via email
                var emailContent = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
                        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                        .header {{ text-align: center; margin-bottom: 30px; }}
                        .logo {{ font-size: 24px; font-weight: bold; color: #007bff; }}
                        .content {{ line-height: 1.6; color: #333; }}
                        .code {{ font-size: 32px; font-weight: bold; color: #007bff; text-align: center; padding: 20px; background-color: #f8f9fa; border-radius: 8px; margin: 20px 0; letter-spacing: 5px; }}
                        .footer {{ margin-top: 30px; font-size: 12px; color: #666; text-align: center; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <div class='logo'>El7aq</div>
                            <h2>Password Reset Verification</h2>
                        </div>
                        <div class='content'>
                            <p>Hello,</p>
                            <p>We received a request to reset your password for your El7aq account. Use the verification code below to continue:</p>
                            <div class='code'>{verificationCode}</div>
                            <p><strong>This code will expire in 10 minutes for security reasons.</strong></p>
                            <p>If you didn't request this password reset, please ignore this email.</p>
                        </div>
                        <div class='footer'>
                            <p>© 2025 El7aq. All rights reserved.</p>
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";

                try
                {
                    await _emailSender.SendEmailAsync(Input.Email, "El7aq Password Reset - Verification Code", emailContent);
                    _logger.LogInformation("Verification code email sent successfully to: {Email}", Input.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send verification code email to: {Email}", Input.Email);
                    ModelState.AddModelError(string.Empty, "Failed to send verification code. Please try again.");
                    return Page();
                }

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
