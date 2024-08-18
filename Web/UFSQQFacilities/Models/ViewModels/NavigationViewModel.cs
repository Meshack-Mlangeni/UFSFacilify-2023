using UFSQQFacilities.Data;

namespace UFSQQFacilities.Models.ViewModels
{
    public class NavigationViewModel
    {
        public User user { get; set; }
        public IQueryable<Notification> Notifications { get; set; }
        public IQueryable<Booking> Bookings { get; set; }
        public IWrapper Wrapper { get; set; }
    }
}
