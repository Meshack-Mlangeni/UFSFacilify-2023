using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UFSQQFacilities.Data;
using UFSQQFacilities.Models;
using UFSQQFacilities.Models.ViewModels;
using System.Net.Mail;
using System.Net;

namespace UFSQQFacilities.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IWrapper wrapper;
        private readonly string role = "User";
        protected readonly string appEmail = "ufsfacilify@gmail.com",
                                  appEmailPass = "ypshwqponebqeekd";

        public AccountController(UserManager<User> _userManager, SignInManager<User> _signInManager,
            RoleManager<IdentityRole> _roleManager, IWrapper _wrapper)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
            wrapper = _wrapper;
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register(string registerAs = "studentstaff")
        {
            return View(new RegisterViewModel() { RegisterAs = registerAs });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                if (await roleManager.FindByNameAsync(role) == null)
                    await roleManager.CreateAsync(new(role));

                User user = new()
                {
                    UserName = registerModel.LastName + (registerModel.IdPassportNumber ?? registerModel.StudentStaffNumber),
                    IdPassportNumber = registerModel.IdPassportNumber,
                    Email = registerModel.EmailAddress,
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    DateJoined = DateTime.Now,
                    StudentStaffNumber = registerModel.StudentStaffNumber,
                    MobilePassword = registerModel.Password
                };
                wrapper.NotificationRepository.Add(new()
                {
                    Message = $"Hello {user.FirstName}, Welcome to UFS QQ facilities.",
                    UserEmail = user.Email
                });

                wrapper.RecoveryRepository.Add(new()
                {
                    SecurityQuestion = registerModel.SecurityQuestion,
                    SecurityAnswer = registerModel.SecurityAnswer,
                    UserEmail = user.Email
                });

                IdentityResult result = await userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    wrapper.Save();


                    var signin_result = await signInManager.PasswordSignInAsync(user, registerModel.Password,
                        isPersistent: false, lockoutOnFailure: false);
                    if (signin_result.Succeeded)
                        return RedirectToAction("Index", "Home");
                }
                else
                    foreach (var error in result.Errors.Select(e => e.Description))
                        ModelState.AddModelError(string.Empty, error);
            }
            return View(registerModel);
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (user.PasswordHash == null)
                    {

                        await userManager.UpdateSecurityStampAsync(user);
                        if ((await userManager.AddPasswordAsync(user, user.MobilePassword)).Succeeded)
                        {
                            user.NormalizedEmail = user.Email.ToUpper();
                            user.NormalizedUserName = user.UserName.ToUpper();
                            await userManager.UpdateAsync(user);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Couldn't sync mobile registered user with application.");
                            return View(model);
                        }
                    }
                    var result = await signInManager.PasswordSignInAsync
                        (user, model.Password, isPersistent: model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        return Redirect(model?.ReturnUrl ?? "/Home/Index");
                    }

                }
            }
            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Recovery(string email)
        {
            if (email != null)
            {
                User user = await userManager.FindByEmailAsync(email);
                if (user != null) return View(new RecoverAccountViewModel()
                {
                    Email = email,
                    SecurityQuestion = wrapper.RecoveryRepository.FindUserQuestion(email)
                });
                else
                {
                    ViewBag.Error = "User was not found, please contact your system administrator";
                    return View();
                }
            }
            else return View();
        }

        [ViewData]
        public string Message { get; set; }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Recovery(RecoverAccountViewModel recovery)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.FindByEmailAsync(recovery.Email);
                if (user != null)
                {
                    if (wrapper.RecoveryRepository.VerifyAnswer(recovery.SecurityAnswer, recovery.Email))
                    {
                        IdentityResult result = await userManager.RemovePasswordAsync(user);
                        if (result.Succeeded)
                        {
                            result = await userManager.AddPasswordAsync(user, recovery.Password);
                            if (result.Succeeded)
                            {

                                wrapper.NotificationRepository.Add(new()
                                {
                                    Message = $"Hello {user.FirstName}, you changed your password on {DateTime.Now}." +
                                    $" If you did not make this change, please call 067 1101 999",
                                    UserEmail = user.Email
                                });
                                wrapper.Save();

                                var signin_result = await signInManager.PasswordSignInAsync(user, recovery.Password,
                                    isPersistent: false, lockoutOnFailure: false);
                                if (signin_result.Succeeded)
                                    return RedirectToAction("Index", "Home");
                            }
                            else
                                ViewBag.Error = "Couldn't recover account, please contact your system administrator.";
                        }
                        else
                            ViewBag.Error = "Couldn't recover account, please contact your system administrator.";
                    }
                }
            }
            return View(recovery);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Edit(string id)
        {
            User user = await userManager.FindByIdAsync(id);
            ViewBag.isManager = (await userManager.GetUsersInRoleAsync("Manager")).Any(u => u.Id == user.Id);
            return View(new EditViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IdPassportNumber = user.IdPassportNumber,
                StudentStaffNumber = user.StudentStaffNumber,
                Username = user.UserName
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            User user = await userManager.FindByIdAsync(model.Id);
            if (user != null && ModelState.IsValid)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.StudentStaffNumber = model.StudentStaffNumber;
                user.IdPassportNumber = model.IdPassportNumber;
                user.DateModified = DateTime.Now;

                if (model.Password != null)
                {
                    if ((await userManager.RemovePasswordAsync(user)).Succeeded)
                        if ((await userManager.AddPasswordAsync(user, model.Password)).Succeeded)
                            if ((await userManager.UpdateAsync(user)).Succeeded)
                                if (User.IsInRole("Administrator"))
                                    return RedirectToAction("Users");
                                else return RedirectToAction("Index", "Home");
                }

                if ((await userManager.UpdateAsync(user)).Succeeded)
                    if (User.IsInRole("Administrator"))
                        return RedirectToAction("Users", "Admin");
                    else return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }


        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
