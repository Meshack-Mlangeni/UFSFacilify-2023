using Microsoft.EntityFrameworkCore;
using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public class FacilityRepository : RepoBase<Facility>, IFacilityRepository
    {
        public FacilityRepository(AppDbContext context) : base(context) { }

        public IQueryable<Facility> GetFacilitiesWithCategoriesAndImages()
        {
            return context.Facilities.Include(c => c.Categories).Include(i => i.Images).Include(r => r.Reviews);
        }

        public IQueryable<Facility> GetFacilitiesWithCategory()
        {
            return context.Facilities.Include(c => c.Categories);
        }

        public IQueryable<Facility> GetFacilityWithBookingAndCategory()
        {
            return context.Facilities.Include(b => b.Bookings).Include(c => c.Categories);
        }

        public IQueryable<Facility> GetFacilityWithBookingAndCategoryById(string userEmail)
        {
            return context.Facilities.Include(b => b.Bookings.Where(u => u.UserEmail == userEmail)).Include(c => c.Categories);
        }

        public Facility GetFacilityWithCategoriesById(int id)
        {
            return GetFacilitiesWithCategoriesAndImages().FirstOrDefault(f => f.FacilityId == id);
        }

        public Facility GetFacilityWithReviewById(int id)
        {
            return context.Facilities.Include(r => r.Reviews).FirstOrDefault(f => f.FacilityId == id);
        }

        public IQueryable<Facility> GetFacilityWithReviews()
        {
            return context.Facilities.Include(r => r.Reviews);
        }
    }
}
