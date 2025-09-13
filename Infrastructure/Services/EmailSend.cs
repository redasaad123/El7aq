using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services
{
    public class EmailService : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await SendEmailInternalAsync(email, subject, htmlMessage, isHtml: true);
        }

        private async Task SendEmailInternalAsync(string toEmail, string subject, string body, bool isHtml)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
                var smtpPort = _configuration.GetValue<int>("EmailSettings:SmtpPort", 587);
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "mentorlink.app@gmail.com";
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "atky pwli ueep eufu";
                var senderName = _configuration["EmailSettings:SenderName"] ?? "El7aq Support";
                var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? "El7aq.app@gmail.com";

                using var smtp = new SmtpClient(smtpServer, smtpPort);
                smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtp.EnableSsl = true;

                using var mail = new MailMessage();
                mail.From = new MailAddress(senderEmail, senderName);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = isHtml;

                await smtp.SendMailAsync(mail);
                _logger.LogInformation("Email sent successfully to {Email} with subject: {Subject} from {SenderName}", toEmail, subject, senderName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email} with subject: {Subject}", toEmail, subject);
                throw; // Re-throw to let calling code handle the failure
            }
        }
    }
}
