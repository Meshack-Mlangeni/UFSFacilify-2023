using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public class FavouriteRepository: RepoBase<Favourite>, IFavouriteRepository
    {
        public FavouriteRepository(AppDbContext context):base(context) { }
    }
}
