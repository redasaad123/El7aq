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
                    // Very relaxed password requirements for testing
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 1;
                    options.Password.RequiredUniqueChars = 1;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddLogging();
            // Register custom claims principal factory
            // builder.Services.AddScoped<IUserClaimsPrincipalFactory<AppUsers>, ApplicationClaimsPrincipalFactory>();

            // Application services and unit of work registrations
            builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            builder.Services.AddScoped<ITripService, TripService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IPassengerHelperService, PassengerHelperService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddTransient<IEmailSender, EmailSend2>();


            builder.Services.AddControllersWithViews();
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MapperConfig>();
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

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<AppUsers>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                string adminRole = "Admin";
                string adminEmail = "admin@test.com";
                string adminPassword = "Admin@123";

                // 1. Add Role if not exists
                if (!await roleManager.RoleExistsAsync(adminRole))
                {
                    await roleManager.CreateAsync(new IdentityRole(adminRole));
                }

                // 2. Add Admin User if not exists
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new AppUsers
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FirstName = "Basmala",
                        LastName = "Mohammed"
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, adminRole);
                    }
                }
            }
            app.Run();
        }
    }
}
