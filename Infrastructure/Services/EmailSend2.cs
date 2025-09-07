using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services
{
    public class EmailSend2 : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("taleon18249456@gmail.com", "qcfsmhunnwnsidpu");
                    smtp.EnableSsl = true;

                    var mail = new MailMessage("taleon18249456@gmail.com", email, subject, htmlMessage);
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null) Console.WriteLine(ex.InnerException.Message);
            }
            ;
            return Task.CompletedTask;
        }
    }
}
