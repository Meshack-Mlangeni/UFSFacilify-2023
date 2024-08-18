using Microsoft.EntityFrameworkCore;
using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public class CategoryRepository: RepoBase<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context): base(context) { }

        public IQueryable<Category> GetCategoriesWithFacilities()
        {
            throw new NotImplementedException();
        }
    }
}
