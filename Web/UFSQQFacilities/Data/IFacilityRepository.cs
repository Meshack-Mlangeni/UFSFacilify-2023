using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public interface IFacilityRepository : IRepoBase<Facility>
    {
        IQueryable<Facility> GetFacilitiesWithCategoriesAndImages();
        Facility GetFacilityWithCategoriesById(int id);
        IQueryable<Facility> GetFacilityWithBookingAndCategoryById(string userEmail);
        IQueryable<Facility> GetFacilityWithBookingAndCategory();
        Facility GetFacilityWithReviewById(int id);
        IQueryable<Facility> GetFacilityWithReviews();
        IQueryable<Facility> GetFacilitiesWithCategory();
    }
}
