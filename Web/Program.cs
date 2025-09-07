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
        public static async Task Main(string[] args)
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
                    options.SignIn.RequireConfirmedAccount = false;
                    
            // Relaxed password requirements for testing
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 3;
            options.Password.RequiredUniqueChars = 0;
                    
                    // User settings
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    
                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddLogging();
            
            // Register custom claims principal factory
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<AppUsers>, Infrastructure.Services.ApplicationClaimsPrincipalFactory>();

            // Application services and unit of work registrations
            builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            builder.Services.AddScoped<ITripService, TripService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IPassengerHelperService, PassengerHelperService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddTransient<IEmailSender, EmailSend2>();
            builder.Services.AddScoped<RoleSeederService>();


            builder.Services.AddControllersWithViews();
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MapperConfig>();
            });

            // Add authorization policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("StaffOnly", policy => policy.RequireRole("Staff"));
                options.AddPolicy("ManagerOrStaff", policy => policy.RequireRole("Manager", "Staff"));
                options.AddPolicy("DriverOrStaff", policy => policy.RequireRole("Driver", "Staff"));
                options.AddPolicy("StaffOrManager", policy => policy.RequireRole("Staff", "Manager"));
                options.AddPolicy("PassengerOrStaff", policy => policy.RequireRole("Passenger", "Staff"));
                options.AddPolicy("Driver", policy => policy.RequireRole("Driver"));
            });

            builder.Services.AddScoped<IEmailSend, EmailSend >();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IPayPalService, PayPalService>();
            builder.Services.AddRazorPages();

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

            
            app.Run();
        }
    }
}
