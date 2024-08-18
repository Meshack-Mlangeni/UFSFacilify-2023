using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public class ImageRepository: RepoBase<FacilityImage>, IImageRepository
    {
        public ImageRepository(AppDbContext context):base(context) { }
    }
}
