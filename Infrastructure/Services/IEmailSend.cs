using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEmailSend
    {
        Task SendEmail(string toEmail, string subject, string body);
    }

    public class EmailSend : IEmailSend
    {
        Task IEmailSend.SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("taleon18249456@gmail.com", "qcfsmhunnwnsidpu");
                    smtp.EnableSsl = true;

                    var mail = new MailMessage("taleon18249456@gmail.com", toEmail, subject, body);
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null) Console.WriteLine(ex.InnerException.Message);
            };
            return Task.CompletedTask;
        }
    }
}
