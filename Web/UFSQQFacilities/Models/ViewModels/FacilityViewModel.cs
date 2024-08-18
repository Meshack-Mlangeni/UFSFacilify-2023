namespace UFSQQFacilities.Models.ViewModels
{
    public class FacilityViewModel
    {
        public User _User { get; set; }
        public Facility SelectedFacility { get; set; }
        public IQueryable<Facility> Facilities { get; set; }
        public IQueryable<Favourite> Favourites{ get; set; }
    }
}
