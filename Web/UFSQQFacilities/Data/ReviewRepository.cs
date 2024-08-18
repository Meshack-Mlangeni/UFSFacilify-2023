using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public class ReviewRepository: RepoBase<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context):base(context) { }
    }
}
