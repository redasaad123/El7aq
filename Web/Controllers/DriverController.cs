using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Models;
using Web.Models.Driver;

namespace Web.Controllers
{
    [Authorize(Roles = "Driver,Staff")]
    public class DriverController : Controller
    {
        private readonly UserManager<AppUsers> userManager;
        private readonly IUnitOfWork<DriverProfile> drivernUitOfWork;

        public DriverController(UserManager<AppUsers> userManager , IUnitOfWork<DriverProfile> DrivernUitOfWork)
        {
            this.userManager = userManager;
            drivernUitOfWork = DrivernUitOfWork;
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
                
                // Safely split the name
                var nameParts = model.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (nameParts.Length >= 2)
                {
                    user.FirstName = nameParts[0];
                    user.LastName = string.Join(" ", nameParts.Skip(1));
                }
                else if (nameParts.Length == 1)
                {
                    user.FirstName = nameParts[0];
                    user.LastName = "";
                }
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
