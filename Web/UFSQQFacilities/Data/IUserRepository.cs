using UFSQQFacilities.Data.DataAccess;
using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public interface IUserRepository : IRepoBase<User>
    {
        IQueryable<User> GetUsersWithOptions(QueryOptions<User> options, IQueryable<User> users);
    }
}
