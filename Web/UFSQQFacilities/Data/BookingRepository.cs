using Microsoft.EntityFrameworkCore;
using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public class BookingRepository : RepoBase<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context) { }

        public IQueryable<Booking> GetBookingsWithFacilityAndCategory(string userEmail)
        {
            return context.Bookings.Include(f => f.Facilities).Include(c => c.Categories).Where(b => b.UserEmail == userEmail);
        }

        public IQueryable<Booking> GetAllBookingsWithFacilityAndCategory()
        {
            return context.Bookings.Include(f => f.Facilities).Include(c => c.Categories);
        }
    }
}
