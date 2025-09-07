
namespace Infrastructure.Services
{
    public interface IEmailSend
    {
        Task SendEmail(string toEmail, string subject, string body);
    }
}