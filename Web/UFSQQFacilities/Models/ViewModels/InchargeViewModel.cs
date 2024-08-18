namespace UFSQQFacilities.Models.ViewModels
{
    public class InchargeViewModel
    {
        public IQueryable<User> Users { get; set; }
        public IList<User> Managers { get; set; }
        public IQueryable<Facility> Facilities{ get; set; }
    }
}
