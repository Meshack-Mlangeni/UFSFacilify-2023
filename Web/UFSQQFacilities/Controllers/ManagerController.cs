using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using UFSQQFacilities.Data;
using UFSQQFacilities.Models;
using UFSQQFacilities.Models.ViewModels;

namespace UFSQQFacilities.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IWrapper wrapper;
        public ManagerController(UserManager<User> _userManager, SignInManager<User> _signInManager,
            RoleManager<IdentityRole> _roleManager, IWrapper _wrapper)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
            wrapper = _wrapper;
        }

        [TempData]
        public string Message { get; set; }

        [Authorize(Roles = "Manager")]
        public IActionResult Reviews(int id = 1)
        {

            return View(new FacilityViewModel
            {
                Facilities = wrapper.FacilityRepository.GetFacilityWithReviews(),
                SelectedFacility = wrapper.FacilityRepository.GetFacilityWithReviewById(id)
            });
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Manager")]
        public IActionResult Delete(int id)
        {
            Facility facility = wrapper.FacilityRepository.FindById(id);
            if (facility != null)
            {
                foreach (var facilityImage in wrapper.ImageRepository.FindAll().Where(i => i.FacilityId == facility.FacilityId))
                {
                    wrapper.ImageRepository.Delete(facilityImage);
                    wrapper.Save();
                }
                wrapper.FacilityRepository.Delete(facility);
                wrapper.Save();
                Message = $"{facility.Name} was successfully deleted";
                return RedirectToAction("Facilities", "Home");
            }
            Message = $"Facility couldn't be deleted, please contact system administrator";
            return RedirectToAction("Facilities", "Home");
        }

        [Authorize(Roles = "Administrator, Manager")]
        public IActionResult Edit(int id)
        {
            return View(wrapper.FacilityRepository.FindById(id));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Manager")]
        public IActionResult Edit(Facility facility, string categories)
        {
            if (ModelState.IsValid)
            {
                wrapper.FacilityRepository.Update(facility);

                if (categories != null)
                    Array.ForEach(categories.Split(","), (category) =>
                        {
                            Category _category = new()
                            {
                                CategoryName = category.Trim(),
                                FacilityId = facility.FacilityId
                            };
                            wrapper.CategoryRepository.Add(_category);
                        });

                wrapper.Save();
                Message = $"{facility.Name} was successfully updated";
                return RedirectToAction("Facilities", "Home");
            }
            Message = $"Facility couldn't be updated, please contact system administrator";
            return View(facility);
        }
        [HttpGet]
        public IActionResult Transactions()
        {
            return View(wrapper.TransactionRepository.GetTransactionsWithBookings());
        }

        [HttpGet]
        public async Task<IActionResult> Incharge()
        {
            PopulateDll();
            return View(new InchargeViewModel()
            {
                Users = userManager.Users.Where(u => u.UserName.ToLower() != "admin"),
                Managers = await userManager.GetUsersInRoleAsync("Manager"),
                Facilities = wrapper.FacilityRepository.FindAll()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Incharge(string id, int facility)
        {
            if (id != null && facility != 0)
            {
                if (await roleManager.FindByNameAsync("Incharge") == null)
                    await roleManager.CreateAsync(new("Incharge"));

                var user = await userManager.FindByIdAsync(id);
                var _facility = wrapper.FacilityRepository.FindById(facility);

                _facility.SecurityEmail = user.Email;
                wrapper.FacilityRepository.Update(_facility);
                wrapper.Save();
                if ((await userManager.AddToRoleAsync(user, "Incharge")).Succeeded)
                {
                    if (await userManager.IsInRoleAsync(user, "User"))
                        await userManager.RemoveFromRoleAsync(user, "User");

                    wrapper.Save();
                    Message = $"{user.FirstName} {user.LastName} was assigned as a security for {_facility.Name}.";
                    return RedirectToAction("Incharge", "Manager");
                }
            }
            ModelState.AddModelError("", "An error occured, please contact the administrator.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveIncharge(int id)
        {
            if (id != 0)
            {
                var _facility = wrapper.FacilityRepository.FindById(id);
                var user = await userManager.FindByEmailAsync(_facility.SecurityEmail);
                _facility.SecurityEmail = null;
                wrapper.FacilityRepository.Update(_facility);

                await userManager.RemoveFromRoleAsync(user, "Incharge");
                wrapper.Save();

                Message = $"User was removed as a security for {_facility.Name}.";
                return RedirectToAction("Incharge", "Manager");
            }
            ModelState.AddModelError("", "An error occured, please contact the administrator.");
            return View();
        }

        private void PopulateDll(object selected_value = null)
        {
            ViewBag.Facilities = new SelectList(wrapper.FacilityRepository.FindAll().Where(f => f.SecurityEmail == null), "FacilityId", "Name", selected_value);
        }
    }

}

