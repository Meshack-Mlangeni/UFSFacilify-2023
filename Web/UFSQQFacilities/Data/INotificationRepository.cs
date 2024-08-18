using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public interface INotificationRepository : IRepoBase<Notification>
    {
        IQueryable<Notification> FindUserNotifications(User user);
    }
}
