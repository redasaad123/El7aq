using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CodeVerificationService : ICodeVerificationService
    {
        private readonly ConcurrentDictionary<string, (string Code, DateTime ExpiryTime)> _verificationCodes;
        private readonly Random _random;

        public CodeVerificationService()
        {
            _verificationCodes = new ConcurrentDictionary<string, (string, DateTime)>();
            _random = new Random();
        }

        public async Task<string> GenerateVerificationCodeAsync(string email)
        {
            // Generate a 6-digit code
            var code = _random.Next(100000, 999999).ToString();
            
            // Set expiration time to 10 minutes from now
            var expiryTime = DateTime.UtcNow.AddMinutes(10);
            
            // Store the code with expiration time
            _verificationCodes.AddOrUpdate(email.ToLowerInvariant(), 
                (code, expiryTime), 
                (key, existingValue) => (code, expiryTime));

            return await Task.FromResult(code);
        }

        public async Task<bool> ValidateVerificationCodeAsync(string email, string code)
        {
            var emailKey = email.ToLowerInvariant();
            
            if (!_verificationCodes.TryGetValue(emailKey, out var storedData))
            {
                return await Task.FromResult(false);
            }

            // Check if code matches and is not expired
            var isValid = storedData.Code == code && DateTime.UtcNow <= storedData.ExpiryTime;
            
            if (isValid)
            {
                // Remove the code after successful validation
                _verificationCodes.TryRemove(emailKey, out _);
            }

            return await Task.FromResult(isValid);
        }

        public async Task<bool> IsCodeExpiredAsync(string email)
        {
            var emailKey = email.ToLowerInvariant();
            
            if (!_verificationCodes.TryGetValue(emailKey, out var storedData))
            {
                return await Task.FromResult(true);
            }

            return await Task.FromResult(DateTime.UtcNow > storedData.ExpiryTime);
        }

        public async Task InvalidateCodeAsync(string email)
        {
            var emailKey = email.ToLowerInvariant();
            _verificationCodes.TryRemove(emailKey, out _);
            await Task.CompletedTask;
        }
    }
}
