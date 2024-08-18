using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UFSQQFacilities.Data;
using UFSQQFacilities.Models;
using UFSQQFacilities.Models.ViewModels;

namespace UFSQQFacilities.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IWrapper wrapper;
        public HomeController(UserManager<User> _userManager, SignInManager<User> _signInManager,
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
        public async Task<IActionResult> Index()
        {
            var _user = await userManager.FindByNameAsync(User.Identity.Name);
            if (_user != null)
            {
                IQueryable<Booking> userBookings = wrapper.BookingRepository.GetBookingsWithFacilityAndCategory(_user.Email).Where(b => b.DateEnd < DateTime.Now);
                if (userBookings != null)
                    Array.ForEach(userBookings.ToArray(), (Booking b) =>
                    {
                        b.IsValid = false;
                        wrapper.BookingRepository.Update(b);
                        wrapper.Save();
                    });
            }
            else
            {
                await signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            return View(new NavigationViewModel()
            {
                user = _user,
                Notifications = wrapper.NotificationRepository.FindUserNotifications(_user),
                Bookings =
                ((User.Identity.Name.ToLower() == "admin" ||
               (await userManager.IsInRoleAsync(_user, "Manager")) || (await userManager.IsInRoleAsync(_user, "Incharge")))
               ? wrapper.BookingRepository.GetAllBookingsWithFacilityAndCategory().OrderBy(b => b.IsValid) :
               wrapper.BookingRepository.GetBookingsWithFacilityAndCategory(_user.Email).OrderBy(b => b.IsValid)),
                Wrapper = wrapper
            });
        }

        [HttpGet]
        public async Task<IActionResult> ModifyFavourites(int id)
        {
            var _user = await userManager.FindByNameAsync(User.Identity.Name);
            if (_user != null && id != 0)
            {
                if (!wrapper.FavouriteRepository.FindAll().Where(f => f.UserEmail == _user.Email).Any(e => e.FacilityId == id))
                {
                    wrapper.FavouriteRepository.Add(new()
                    {
                        FacilityId = id,
                        UserEmail = _user.Email,
                    });
                    Message = $"Facility added to {_user.FirstName}'s favourites";
                }
                else
                {
                    IQueryable<Favourite> favourite = wrapper.FavouriteRepository.FindAll().Where(i => i.FacilityId == id);
                    Array.ForEach(favourite.ToArray(), (fav) =>
                    {
                        wrapper.FavouriteRepository.Delete(fav);
                        wrapper.Save();
                    });
                    Message = $"Facility removed from {_user.FirstName}'s favourites";
                }
                wrapper.Save();
                return RedirectToAction("Facilities", "Home");
            }
            Message = $"Facility couldn't be added to favourites, please contact the system adminstrator.";
            return RedirectToAction("Facilities", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Facilities(int id = 1)
        {
            var _user = await userManager.FindByNameAsync(User.Identity.Name);
            return View(new FacilityViewModel()
            {
                _User = _user,
                SelectedFacility = wrapper.FacilityRepository.GetFacilityWithCategoriesById(id),
                Facilities = wrapper.FacilityRepository.GetFacilitiesWithCategoriesAndImages(),
                Favourites = wrapper.FavouriteRepository.FindAll().Where(u => u.UserEmail.ToLower()
                == _user.Email.ToLower())
            });
        }
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> BookingHistory()
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<Booking> bookings = (User.Identity.Name.ToLower() == "admin" ||
                (await userManager.IsInRoleAsync(user, "Manager")) || (await userManager.IsInRoleAsync(user, "Incharge"))
                ) ? wrapper.BookingRepository.GetAllBookingsWithFacilityAndCategory().OrderBy(b => b.IsValid) :
                wrapper.BookingRepository.GetBookingsWithFacilityAndCategory(user.Email).OrderBy(b => b.IsValid);
            return View(bookings);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public IActionResult BookingHistory(int id)
        {
            if (id != 0)
            {
                Booking booking = wrapper.BookingRepository.FindById(id);
                booking.Approved = true;
                wrapper.BookingRepository.Update(booking);

                wrapper.NotificationRepository.Add(new()
                {
                    Created = DateTime.Now,
                    IsRead = false,
                    Message = "You booking order was approved by the facility manager. Please proceed to your desired facility.",
                    UserEmail = booking.UserEmail
                });

                wrapper.Save();
                Message = "Booking order status was changed successfully";
                return RedirectToAction("BookingHistory", "Home");
            }
            Message = "Booking order status couldn't be changed, please contact system administrator ";
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            return View(wrapper.NotificationRepository.FindUserNotifications
                 ((await userManager.FindByNameAsync(User.Identity.Name))));
        }

        [HttpPost]
        public async Task<IActionResult> Notifications(int id)
        {
            Notification notification = wrapper.NotificationRepository.FindById(id);
            notification.IsRead = true;
            wrapper.NotificationRepository.Update(notification);
            wrapper.Save();

            return View(wrapper.NotificationRepository.FindUserNotifications
                ((await userManager.FindByNameAsync(User.Identity.Name))));
        }

        [HttpGet]
        public async Task<IActionResult> Review(int id)
        {
            return View(new Review()
            {
                FacilityId = id,
                UserEmail = (await userManager.FindByNameAsync(User.Identity.Name)).Email,
            });
        }

        [HttpPost]
        public IActionResult Review(Review review)
        {
            if (ModelState.IsValid)
            {
                wrapper.ReviewRepository.Add(review);
                wrapper.Save();
                Message = "Review added successfully";
                return RedirectToAction("Facilities", "Home");
            }
            Message = "Review couldn't be added. Please contact system administrator";
            return View(review);
        }

        public IActionResult AddRemoveFavourite(int id)
        {
            // wrapper.ReviewRepository.Add(review);
            wrapper.Save();
            Message = "Review added successfully";
            return RedirectToAction("Facilities", "Home");

        }

    }
}
