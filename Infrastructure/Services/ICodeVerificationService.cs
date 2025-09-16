using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface ICodeVerificationService
    {
        Task<string> GenerateVerificationCodeAsync(string email);
        Task<bool> ValidateVerificationCodeAsync(string email, string code);
        Task<bool> IsCodeExpiredAsync(string email);
        Task InvalidateCodeAsync(string email);
    }
}
