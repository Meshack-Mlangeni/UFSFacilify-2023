using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;
using Stripe.Checkout;
using UFSQQFacilities.Data;
using UFSQQFacilities.Models;

namespace UFSQQFacilities.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IWrapper wrapper;
        private readonly string random_char = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        public BookingController(UserManager<User> _userManager, SignInManager<User> _signInManager,
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
        public async Task<IActionResult> Book(int id)
        {
            ViewBag.Facility = wrapper.FacilityRepository.FindById(id);
            PopulateDll(id);
            return View(new Booking()
            {
                Approved = false,
                FacilityId = id,
                UserEmail = (await userManager.FindByNameAsync(User.Identity.Name)).Email,
                IsValid = true
            });
        }

        [HttpGet]
        public IActionResult Success(int id, bool approved = true)
        {
            Booking booking = wrapper.BookingRepository.FindById(id);
  
            booking.Approved = approved;
            booking.BookingPass = "web-" + new string(Enumerable.Repeat(random_char, 5).Select(ran_char => ran_char[new Random().Next(ran_char.Length - 1)]).ToArray()) + booking.UserEmail[0];
            wrapper.BookingRepository.Update(booking);
            wrapper.Save();
            if (approved)
            {
                wrapper.TransactionRepository.Add(new()
                {
                    BookingId = booking.BookingId,
                    Date = DateTime.Now,
                    Success = true,
                    UserEmail = booking.UserEmail
                });
                wrapper.Save();
                wrapper.NotificationRepository.Add(new()
                {
                    Created = DateTime.Now,
                    IsRead = false,
                    Message = $"The payment for booking {wrapper.FacilityRepository.FindById(booking.FacilityId).Name} " +
                    $"was successful, thank you for making a booking",
                    UserEmail = booking.UserEmail
                });
                wrapper.Save();
            }
            else
            {
                wrapper.NotificationRepository.Add(new()
                {
                    Created = DateTime.Now,
                    IsRead = false,
                    Message = $"Order for booking {wrapper.FacilityRepository.FindById(booking.FacilityId).Name} " +
                             $"was successful but not yet approved, please make a payment to secure your place.",
                    UserEmail = booking.UserEmail
                });
                wrapper.Save();
            }
            return View(booking);
        }

        [HttpGet]
        public IActionResult Cancel(int id)
        {
            Booking booking = wrapper.BookingRepository.FindById(id);

            wrapper.TransactionRepository.Add(new()
            {
                BookingId = booking.BookingId,
                Date = DateTime.Now,
                Success = false,
                UserEmail = booking.UserEmail
            });
            wrapper.Save();
            wrapper.NotificationRepository.Add(new()
            {
                Created = DateTime.Now,
                IsRead = false,
                Message = $"The payment for booking {wrapper.FacilityRepository.FindById(booking.FacilityId).Name} " +
                $"was unsuccessful, please try again later",
                UserEmail = booking.UserEmail
            });

            wrapper.BookingRepository.Delete(booking);
            wrapper.Save();
            return View(booking);
        }


        [HttpGet]
        public IActionResult CheckOut(int id)
        {
            string appDomain = "http://" + HttpContext.Request.Host.ToUriComponent();
            Booking booking = wrapper.BookingRepository.FindById(id);
            if (booking != null)
            {
                try
                {
                    var options = new SessionCreateOptions
                    {
                        CancelUrl = appDomain + $"/Booking/Cancel/{id}",
                        SuccessUrl = appDomain + $"/Booking/Success/{id}",
                        LineItems = new List<SessionLineItemOptions>
                            {
                                 new SessionLineItemOptions
                                 {
                                     PriceData = new SessionLineItemPriceDataOptions()
                                     {
                                         UnitAmount = (long)(100 * (double)wrapper.FacilityRepository.FindById(booking.FacilityId).Price),
                                         Currency = "zar",
                                         ProductData = new SessionLineItemPriceDataProductDataOptions()
                                         {
                                             Name = wrapper.FacilityRepository.FindById(booking.FacilityId).Name,
                                             Description = $"User has booked {wrapper.FacilityRepository.FindById(booking.FacilityId).Name} to use " +
                                                           $"{wrapper.CategoryRepository.FindById(booking.CategoryId).CategoryName}."
                                         }
                                     },
                                     Quantity = 1,
                                 },
                            },
                        Mode = "payment",
                        CustomerEmail = booking.UserEmail
                    };
                    var service = new SessionService();
                    Session session = service.Create(options);
                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);
                }
                catch
                {
                    Message = "An error occured when trying to checkout and your booking was reversed, please contact your system administrator";
                    wrapper.BookingRepository.Delete(booking);
                    wrapper.Save();
                    return RedirectToAction("Facilities", "Home");
                }
            }
            Message = "An error occured when trying to checkout, please contact your system admin";
            return RedirectToAction("Facilities", "Home");
        }

        [HttpPost]
        public IActionResult Book(Booking booking)
        {
            if (ModelState.IsValid)
            {
                if (booking.DateStart > DateTime.Now)
                {
                    booking.DateEnd = booking.DateStart + TimeSpan.FromMinutes(30);
                    wrapper.BookingRepository.Add(booking);
                    wrapper.Save();
                    return RedirectToAction("ConfirmPaymentOption", "Booking", booking);
                }
                else if (wrapper.BookingRepository.FindAll().Any(b => b.DateStart.Equals(DateTime.Now) && b.CategoryId == booking.CategoryId))
                    ModelState.AddModelError("", "There is already a booking");
                else
                    ModelState.AddModelError("", "Bookings cannot be before the date today");
            }
            ViewBag.Facility = wrapper.FacilityRepository.FindById(booking.FacilityId);
            PopulateDll(booking.FacilityId);
            return View(booking);
        }

        public IActionResult ConfirmPaymentOption(Booking booking)
        {
            ViewBag.Facility = wrapper.FacilityRepository.FindById(booking.FacilityId);
            ViewBag.Category = wrapper.CategoryRepository.FindById(booking.CategoryId).CategoryName;
            return View(booking);
        }

        [HttpPost]
        public IActionResult OrderConfirmation(int id, string type)
        {
            if (type != null)
            {
                if (type == "manual")
                {
                    Booking booking = wrapper.BookingRepository.FindById(id);
                    booking.Approved = false;
                    wrapper.BookingRepository.Update(booking);
                    wrapper.Save();
                    Message = "Booking was placed successfully";

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                Message = "There was an error making a booking, please contact your system administrator";
                return RedirectToAction("ConfirmPaymentOption", wrapper.BookingRepository.FindById(id));
            }
        }

        private void PopulateDll(int facilityId, object selected_value = null)
        {
            ViewBag.Categories = new SelectList(wrapper.CategoryRepository.FindAll().
                Where(c => c.FacilityId == facilityId), "CategoryId", "CategoryName", selected_value);
        }
    }
}
