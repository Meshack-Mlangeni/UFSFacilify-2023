using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public interface ICategoryRepository: IRepoBase<Category>
    {
        IQueryable<Category> GetCategoriesWithFacilities();
    }
}
