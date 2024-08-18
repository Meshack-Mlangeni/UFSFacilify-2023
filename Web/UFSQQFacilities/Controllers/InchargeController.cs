using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UFSQQFacilities.Data;
using UFSQQFacilities.Models;
using UFSQQFacilities.Models.ViewModels;

namespace UFSQQFacilities.Controllers
{
    [Authorize(Roles ="Incharge")]
    public class InchargeController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IWrapper wrapper;
        public InchargeController(UserManager<User> _userManager, SignInManager<User> _signInManager,
            RoleManager<IdentityRole> _roleManager, IWrapper _wrapper)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
            wrapper = _wrapper;
        }

        [TempData]
        public string Message { get; set; }

        [HttpGet]
        public IActionResult ChangeStatus(int id)
        {
            return View(new ChangeStatusViewModel()
            {
                Bookings = wrapper.BookingRepository.FindAll().Where(b => b.FacilityId == id && b.DateEnd.Date > DateTime.Now.Date),
                AssignedFacility = wrapper.FacilityRepository.FindById(id)
            });
        }

        [HttpPost]
        [ActionName("ChangeStatus")]
        public IActionResult ChangeStatusPost(int id)
        {
            if (ModelState.IsValid && id != 0)
            {
                Facility facility = wrapper.FacilityRepository.FindById(id);
                facility.Available = !facility.Available;
                wrapper.FacilityRepository.Update(facility);
                wrapper.Save();
                Message = "Facility status has been changed successfully";
                return RedirectToAction("Facilities", "Home");
            }
            Message = "Facility status couldn't be changed, please contact system administrator.";
            return View(id);
        }
    }
}
