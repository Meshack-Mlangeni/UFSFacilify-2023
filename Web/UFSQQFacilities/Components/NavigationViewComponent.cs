using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UFSQQFacilities.Data;
using UFSQQFacilities.Models;
using UFSQQFacilities.Models.ViewModels;

namespace UFSQQFacilities.Components
{
    public class NavigationViewComponent: ViewComponent
    {
        private readonly UserManager<User> userManager;
        private readonly IWrapper wrapper;
        public NavigationViewComponent(UserManager<User> _userManager, IWrapper _wrapper)
        {
            userManager = _userManager;
            wrapper = _wrapper;
        }

        public async Task<IViewComponentResult> InvokeAsync() 
        {
            User _user = await userManager.FindByNameAsync(User.Identity.Name);
            return View(new NavigationViewModel()
            {
                user = _user,
                Notifications = wrapper.NotificationRepository.FindUserNotifications(_user)
            });
        }
    }
}
