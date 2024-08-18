using UFSQQFacilities.Data.DataAccess;
using System.Linq;
using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public class UserRepository : RepoBase<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }
        public IQueryable<User> GetUsersWithOptions(QueryOptions<User> options, IQueryable<User> users)
        {
            IQueryable<User> query = users;
            if (options.HasWhere) query = query.Where(options.Where);

            if (options.HasOrderBy)
                if (options.OrderByDirection == "asc")
                    query = query.OrderBy(options.OrderBy);
                else
                    query = query.OrderByDescending(options.OrderBy);

            if (options.HasPaging)
                query = query.Skip((options.PageNumber - 1) * options.PageSize)
                                 .Take(options.PageSize);
            return query;
        }
    }
}
