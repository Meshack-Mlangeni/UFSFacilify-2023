using UFSQQFacilities.Data.DataAccess;
using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public interface IRepoBase<T>
    {
        void Add(T item);
        void Delete(T item);
        void Update(T item);
        T FindById(int id);
        IQueryable<T> FindAll();
        IQueryable<T> GetWithOptions(QueryOptions<T> options);
    }
}
