using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Core.Entities;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Core.Interfaces;
using Web.Services;
using Microsoft.AspNetCore.Identity;

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
                .AddDefaultIdentity<AppUsers>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Register custom claims principal factory
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<AppUsers>, ApplicationClaimsPrincipalFactory>();

            // Application services and unit of work registrations
            builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            builder.Services.AddScoped<ITripService, TripService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IPassengerHelperService, PassengerHelperService>();

            builder.Services.AddControllersWithViews();

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
