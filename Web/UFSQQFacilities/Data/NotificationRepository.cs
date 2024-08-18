using Microsoft.EntityFrameworkCore;
using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public class NotificationRepository : RepoBase<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context): base(context) {}

        public IQueryable<Notification> FindUserNotifications(User user)
        {
            return context.Notifications.Where(n => n.UserEmail.ToLower() == user.Email.ToLower()); 
        }
    }
}
