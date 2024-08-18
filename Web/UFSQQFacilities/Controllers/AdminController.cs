
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UFSQQFacilities.Data;
using UFSQQFacilities.Data.DataAccess;
using UFSQQFacilities.Models;
using UFSQQFacilities.Models.ViewModels;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Colors;
using System.Linq.Expressions;

namespace UFSQQFacilities.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IWrapper wrapper;
        public AdminController(UserManager<User> _userManager, SignInManager<User> _signInManager,
            RoleManager<IdentityRole> _roleManager, IWrapper _wrapper)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
            wrapper = _wrapper;
        }
        [TempData]
        public string Message { get; set; }

        private readonly int iItemsPerPage = 5;

        [HttpGet]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> Users(string type = "all", int page = 1)
        {
            IQueryable<User> users;
            int iTotalUsers = userManager.Users.Count();
            string orderByDirection = "asc";

            if (type == "all")
            {
                users = wrapper.UserRepository.GetUsersWithOptions(new QueryOptions<User>()
                {
                    OrderBy = e => e.DateJoined,
                    OrderByDirection = orderByDirection,
                    PageSize = iTotalUsers,
                    Where = u => u.UserName.ToLower() != "admin"
                }, userManager.Users);
            }
            else if (type == "managers")
                users = (await userManager.GetUsersInRoleAsync("Manager")).AsQueryable();
            else
                users = (await userManager.GetUsersInRoleAsync("Incharge")).AsQueryable();


            return View(new UserListViewModel()
            {
                type = type,
                Users = users,
                Managers = (await userManager.GetUsersInRoleAsync("Manager")).AsQueryable(),
                PageInfo = new PagingViewInfoModel()
                {
                    CurrentPage = page,
                    ItemsPerPage = iItemsPerPage,
                    TotalItems = iTotalUsers
                }
            });
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddManager(string id)
        {
            User user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!await roleManager.RoleExistsAsync("Manager"))
                    await roleManager.CreateAsync(new("Manager"));

                IdentityResult result = await userManager.AddToRoleAsync(user, "Manager");
                if (result.Succeeded)
                {
                    Message = $"{user.FirstName} was successfully added as manager";
                    Notification notification = new()
                    {
                        UserEmail = user.Email,
                        Created = DateTime.Now,
                        IsRead = false,
                        Message = "Administrator has added you to the management role"
                    };

                    if (await userManager.IsInRoleAsync(user, "User"))
                        await userManager.RemoveFromRoleAsync(user, "User");

                    wrapper.NotificationRepository.Add(notification);
                    wrapper.Save();
                    return RedirectToAction("Users");
                }
            }
            Message = $"{user.FirstName} couldn't be added as manager";
            return RedirectToAction("Users");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> RemoveManager(string id)
        {
            User user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!await roleManager.RoleExistsAsync("Manager"))
                    await roleManager.CreateAsync(new("Manager"));

                IdentityResult result = await userManager.RemoveFromRoleAsync(user, "Manager");
                if (result.Succeeded)
                {
                    Message = $"{user.FirstName} was successfully added as manager";
                    Notification notification = new()
                    {
                        UserEmail = user.Email,
                        Created = DateTime.Now,
                        IsRead = false,
                        Message = "Administrator has removed you from the management role"
                    };
                    wrapper.NotificationRepository.Add(notification);
                    wrapper.Save();
                    return RedirectToAction("Users");
                }
            }
            Message = $"{user.FirstName} was successfully removed as manager";
            return RedirectToAction("Users");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(string id)
        {
            User user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    Message = $"{user.FirstName} was successfully deleted";
                    return RedirectToAction("Users");
                }
            }
            Message = $"User couldn't be deleted";
            return RedirectToAction("Users");
        }

        [HttpGet]
        public async Task<IActionResult> GenerateReport()
        {
            try
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                IList<User> managers = await userManager.GetUsersInRoleAsync("Manager"),
                            incharge = await userManager.GetUsersInRoleAsync("Incharge");

                using (var memoryStream = new MemoryStream())
                {
                    Document document = new Document(new PdfDocument(new PdfWriter(memoryStream)));
                    document.Add(new Paragraph("UFS FACILIFY REPORT").SetTextAlignment(TextAlignment.CENTER).SetFontSize(16));
                    document.Add(new LineSeparator(new SolidLine()));

                    printAdmin(document, user);
                    document.Add(new LineSeparator(new SolidLine()));
                    printAllUsers("MANAGERS", document, managers);
                    printAllUsers("INCHARGES", document, incharge);

                    document.Add(new Paragraph("APPLICATION USERS").AddStyle(new Style().SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10));
                    document.Add(new Paragraph($"1. The application has {userManager.Users.Count()} overall user(s), including **system administrator.").SetFontSize(8));
                    document.Add(new LineSeparator(new SolidLine(.5f)));

                    printFacilities("FACILITIES", document, wrapper.FacilityRepository.GetFacilitiesWithCategory());
                    printBookings(document, wrapper.FacilityRepository.GetFacilityWithBookingAndCategory());
                    printAllTransaction("TRANSACTIONS", document, wrapper.TransactionRepository.FindAll());


                    document.Close();
                    return new FileContentResult(memoryStream.ToArray(), "application/pdf");
                }
            }
            catch
            {
                Message = "There was an error processing the document. please restart the application and try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        #region PrintReportMethods
        private Cell setHeaderCells(string name) => new Cell().SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(name));
        private Cell setContentCells(string name) => new Cell().SetTextAlignment(TextAlignment.JUSTIFIED).SetFontSize(8).Add(new Paragraph(name));
        private void printBookings(Document document, IQueryable<Facility> facilities)
        {
            foreach (var facility in facilities)
            {
                document.Add(new Paragraph(facility.Name.ToUpper() + " BOOKING REPORT").AddStyle(new Style().SetBold()).SetMarginLeft(16f).SetTextAlignment(TextAlignment.LEFT).SetFontSize(8));
                document.Add(new Paragraph($"The facility has {facility.Bookings.Count()} overall successful booking(s)").SetMarginLeft(16f).SetFontSize(8));
                if (facility.Bookings.Count() > 0)
                {
                    Table table = new Table(5);
                    table.SetMarginLeft(16f);
                    table.AddCell(setHeaderCells("User email")); table.AddCell(setHeaderCells("Equipment/Categories booked")); table.AddCell(setHeaderCells("Date Booked"));
                    table.AddCell(setHeaderCells("Time in")); table.AddCell(setHeaderCells("Time out"));
                    foreach (var booking in facility.Bookings)
                    {
                        table.AddCell(setContentCells(booking.UserEmail)); table.AddCell(setContentCells(booking.Categories.CategoryName));
                        table.AddCell(setContentCells(booking.DateStart.ToShortDateString()));
                        table.AddCell(setContentCells(booking.DateStart.ToShortTimeString()));
                        table.AddCell(setContentCells(booking.DateEnd.ToShortTimeString()));
                    }
                    document.Add(table);
                }
            }
        }
        private void printAllTransaction(string v, Document document, IQueryable<Transaction> transactions)
        {
            document.Add(new Paragraph(v).AddStyle(new Style().SetBold().SetMarginTop(16f)).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10));
            if (transactions.Count() == 0)
                document.Add(new Paragraph($"There are no transactions made."));
            else
            {
                Table table = new Table(3);
                table.AddCell(setHeaderCells("User email")); table.AddCell(setHeaderCells("Transaction date")); table.AddCell(setHeaderCells("Was successfull"));
                foreach (var transaction in transactions)
                {
                    table.AddCell(setContentCells(transaction.UserEmail)); table.AddCell(setContentCells(transaction.Date.ToLongDateString()));
                    table.AddCell(setContentCells(transaction.Success.ToString()));
                }
                document.Add(table);
            }
        }
        private void printFacilities(string header, Document document, IQueryable<Facility> facilities)
        {
            document.Add(new Paragraph(header).AddStyle(new Style().SetBold().SetMarginTop(16f)).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10));
            Table table = new Table(6);
            table.AddCell(setHeaderCells("Name")); table.AddCell(setHeaderCells("Equipment/Categories")); table.AddCell(setHeaderCells("Space"));
            table.AddCell(setHeaderCells("Incharge email")); table.AddCell(setHeaderCells("Price per 30 minutes")); table.AddCell(setHeaderCells("Occupied"));
            foreach (var facility in facilities)
            {
                table.AddCell(setContentCells(facility.Name)); table.AddCell(setContentCells(string.Join(',', facility.Categories.Select(r => r.CategoryName))));
                table.AddCell(setContentCells(facility.Space.ToString())); table.AddCell(setContentCells(facility.SecurityEmail ?? "No Incharge assigned"));
                table.AddCell(setContentCells("R" + facility.Price.ToString("0.00"))); table.AddCell(setContentCells((!facility.Available).ToString()));
            }
            document.Add(table);
        }
        private void printAllUsers(string header, Document document, IList<User> users)
        {
            document.Add(new Paragraph(header).AddStyle(new Style().SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10));
            Table table = new Table(4);
            table.AddCell(setHeaderCells("Email")); table.AddCell(setHeaderCells("First name")); table.AddCell(setHeaderCells("Last name"));
            table.AddCell(setHeaderCells("Date joined"));
            foreach (var user in users)
            {
                table.AddCell(setContentCells(user.Email)); table.AddCell(setContentCells(user.FirstName));
                table.AddCell(setContentCells(user.LastName)); table.AddCell(setContentCells(user.DateJoined.ToLongDateString()));
            }
            document.Add(table);
        }
        private void printAdmin(Document document, User user)
        {
            document.Add(new Paragraph("SYSTEM ADMINISTRATOR").AddStyle(new Style().SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10));
            document.Add(new Paragraph($"Admin full name:\t{user.FirstName} {user.LastName}"));
            document.Add(new Paragraph($"Admin Email:\t\t{user.Email}"));
        }
        #endregion

        [Authorize(Roles = "Administrator, Manager")]
        public IActionResult AddFacility()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> AddFacility(AddFacilityViewModel model)
        {
            if (ModelState.IsValid)
            {
                Facility facility = new()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Space = model.Space
                };
                wrapper.FacilityRepository.Add(facility);
                wrapper.Save();

                facility = wrapper.FacilityRepository.FindAll().FirstOrDefault
                    (f => f.Name == model.Name);

                Array.ForEach(model.Categories.Split(","), (category) =>
                {
                    Category _category = new()
                    {
                        CategoryName = category.Trim(),
                        FacilityId = facility.FacilityId
                    };
                    wrapper.CategoryRepository.Add(_category);
                });

                if (model.Images != default)
                    foreach (var image in model.Images)
                    {
                        try
                        {
                            using (var memory = new MemoryStream())
                            {
                                await image.CopyToAsync(memory);
                                FacilityImage facilityImage = new()
                                {
                                    FacilityId = facility.FacilityId,
                                    Image = memory.ToArray()
                                };
                                wrapper.ImageRepository.Add(facilityImage);
                            }
                        }
                        catch { continue; }
                    }

                wrapper.Save();
                Message = "Facility was successfully added.";
                return RedirectToAction("Facilities", "Home");
            }
            return View();
        }


    }
}
