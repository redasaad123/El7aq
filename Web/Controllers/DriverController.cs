using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Web.Models;
using Web.Models.Driver;

namespace Web.Controllers
{
    [Authorize]
    public class DriverController : Controller
    {
        private readonly UserManager<AppUsers> userManager;
        private readonly IUnitOfWork<DriverOrder> driverOrderUnitOfWork;
        private readonly IUnitOfWork<DriverProfile> drivernUitOfWork;

        public DriverController(UserManager<AppUsers> userManager,IUnitOfWork<DriverOrder> DriverOrderUnitOfWork, IUnitOfWork<DriverProfile> DrivernUitOfWork)
        {
            this.userManager = userManager;
            driverOrderUnitOfWork = DriverOrderUnitOfWork;
            drivernUitOfWork = DrivernUitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);

            var drivers =await driverOrderUnitOfWork.Entity.GetAllAsyncAsQuery().Select(x=>x.DriverId).ToListAsync();
            var driverProfile = drivernUitOfWork.Entity.Find(d => d.UserId == user.Id);
            var index = drivers.IndexOf(driverProfile.Id);


            var model = new HomeViewModel
            {
                Index = index - 1,
            };

            return View(model);


        }

        public async Task<IActionResult> Account()
        {
            var user = await userManager.GetUserAsync(User);
            var driverProfile = drivernUitOfWork.Entity.Find(d=> d.UserId == user.Id);

            if (driverProfile == null)
            {
                return RedirectToAction("Create");
            }



            var driver = new DriverProfileViewModel
            {
                LicenseNumber = driverProfile.LicenseNumber,
                CarNumber = driverProfile.CarNumber,
                Name = user.FirstName + " " + user.LastName,
                Email = user.Email,
                Phone = user.PhoneNumber,
            };

            return View(driver);

        }


        public async Task<IActionResult> UpdateDriver()
        {

            var user = await userManager.GetUserAsync(User);
            var driverProfile = drivernUitOfWork.Entity.Find(d => d.UserId == user.Id);


            var driver = new DriverProfileViewModel
            {
                LicenseNumber = driverProfile.LicenseNumber,
                CarNumber = driverProfile.CarNumber,
                Name = user.FirstName + " " + user.LastName,
                Email = user.Email,
                Phone = user.PhoneNumber,
            };

            return View(driver);
        }


        [HttpPost]

        public async Task<IActionResult> UpdateDriver(DriverProfileViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                var driverProfile = drivernUitOfWork.Entity.Find(d => d.UserId == user.Id);
                if (driverProfile == null)
                {
                    return RedirectToAction("Create");
                }
                driverProfile.CarNumber = model.CarNumber;
                driverProfile.LicenseNumber = model.LicenseNumber;
                user.PhoneNumber = model.Phone;
                user.LastName = model.Name.Split(' ')[0];
                user.FirstName = model.Name.Split(' ')[1];
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    drivernUitOfWork.Entity.Update(driverProfile);
                    await drivernUitOfWork.SaveAsync();
                    return RedirectToAction("Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View("Account", model);

        }
    }
}
