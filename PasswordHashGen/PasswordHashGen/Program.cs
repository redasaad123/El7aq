using Microsoft.AspNetCore.Identity;
using Core.Entities;

class Program
{
    static void Main()
    {
        var passwordHasher = new PasswordHasher<AppUsers>();
        var password = "Aa@2004";
        var user = new AppUsers();
        var hashedPassword = passwordHasher.HashPassword(user, password);
        
        Console.WriteLine($"Password: {password}");
        Console.WriteLine($"Hashed Password: {hashedPassword}");
        Console.WriteLine();
        Console.WriteLine("SQL Update Query:");
        Console.WriteLine($"UPDATE [security].[Users] SET [PasswordHash] = '{hashedPassword}' WHERE [Id] = 'your-user-id';");
    }
}