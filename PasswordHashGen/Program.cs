using Microsoft.AspNetCore.Identity;
using Core.Entities;

namespace PasswordHashGen
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a password hasher instance
            var passwordHasher = new PasswordHasher<AppUsers>();
            
            // The password to hash
            string password = "Aa@2004";
            
            // Create a dummy user (we only need it for the hasher type)
            var user = new AppUsers();
            
            // Hash the password
            string hashedPassword = passwordHasher.HashPassword(user, password);
            
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Hashed Password: {hashedPassword}");
            Console.WriteLine();
            Console.WriteLine("SQL Update Query:");
            Console.WriteLine($"UPDATE [security].[Users] SET [PasswordHash] = '{hashedPassword}' WHERE [Id] = 'your-user-id';");
        }
    }
}