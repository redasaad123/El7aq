using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using Infrastructure.Services;

namespace Web.Controllers
{
    public class TestEmailController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly ICodeVerificationService _codeVerificationService;
        private readonly ILogger<TestEmailController> _logger;

        public TestEmailController(IEmailSender emailSender, ICodeVerificationService codeVerificationService, ILogger<TestEmailController> logger)
        {
            _emailSender = emailSender;
            _codeVerificationService = codeVerificationService;
            _logger = logger;
        }

        public async Task<IActionResult> TestEmail(string email = "test@example.com")
        {
            try
            {
                var code = await _codeVerificationService.GenerateVerificationCodeAsync(email);
                
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
                            <div class='code'>{code}</div>
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

                await _emailSender.SendEmailAsync(email, "El7aq Password Reset - Verification Code (TEST)", emailContent);
                
                return Json(new { success = true, message = $"Test email sent to {email} with code: {code}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send test email to: {Email}", email);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
