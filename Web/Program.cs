using Core;
using Core.Entities;
using Core.Interfaces;
using Infrastructure;
using Infrastructure.Mapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Web.Services;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services
                .AddDefaultIdentity<AppUsers>(options => 
                {
                    // Sign-in settings
                    options.SignIn.RequireConfirmedAccount = false;
                    
                    // Password settings
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 1;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    
                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;
                    
                    // User settings
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = true;
                    
                    // Token settings for password reset
                    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            // Configure Data Protection for token expiration
            builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => 
                opts.TokenLifespan = TimeSpan.FromHours(24));
            builder.Services.AddLogging();
            // Register custom claims principal factory
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<AppUsers>, Infrastructure.Services.ApplicationClaimsPrincipalFactory>();
            builder.Configuration.AddEnvironmentVariables();
            // Application services and unit of work registrations
            builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            builder.Services.AddScoped<ITripService, TripService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IPassengerHelperService, PassengerHelperService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IRoleRoutingService, RoleRoutingService>();
            builder.Services.AddScoped<RoleSeederService>();

            builder.Services.AddTransient<IEmailSender, EmailService>();
            builder.Services.AddTransient<IGeolocationService, OpenStreetMapAdapter>();
            builder.Services.AddSingleton<ICodeVerificationService, CodeVerificationService>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MapperConfig>();
            });

            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IPayPalService, PayPalService>();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Ensure authentication runs before authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // Seed roles on startup to ensure required roles exist in real environments
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeederService>();
                    roleSeeder.SeedRolesAsync().GetAwaiter().GetResult();
                }
                catch
                {
                    // Ignore seeding failures to avoid blocking app startup; errors will be logged inside the seeder
                }
            }

            app.Run();
        }
    }
}