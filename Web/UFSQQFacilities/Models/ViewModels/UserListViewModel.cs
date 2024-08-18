namespace UFSQQFacilities.Models.ViewModels
{
    public class UserListViewModel
    {
        public string type { get; set; }
        public IQueryable<User> Incharges { get; set; }
        public IQueryable<User> Managers { get; set; }
        public IQueryable<User> Users { get; set; }
        public PagingViewInfoModel PageInfo { get; set; }
    }
}
