using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public interface IBookingRepository: IRepoBase<Booking>
    {
        IQueryable<Booking> GetBookingsWithFacilityAndCategory(string userEmail);
        IQueryable<Booking> GetAllBookingsWithFacilityAndCategory();
    }
}
