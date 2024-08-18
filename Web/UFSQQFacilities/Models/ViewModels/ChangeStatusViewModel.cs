namespace UFSQQFacilities.Models.ViewModels
{
    public class ChangeStatusViewModel
    {
        public IQueryable<Booking> Bookings{ get; set; }
        public Facility AssignedFacility{ get; set; }
    }
}
