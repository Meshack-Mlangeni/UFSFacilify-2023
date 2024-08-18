using System.Linq;
using UFSQQFacilities.Data.DataAccess;
using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public abstract class RepoBase<T> : IRepoBase<T> where T : class
    {
        protected AppDbContext context;
        public RepoBase(AppDbContext _context)
        {
            context = _context;
        }
        public void Add(T item) => context.Add(item);
        public void Delete(T item) => context.Remove(item);
        public IQueryable<T> FindAll() => (IQueryable<T>)context.Set<T>();
        public T FindById(int id) => context.Set<T>().Find(id);
        public void Update(T item) => context.Set<T>().Update(item);

        public IQueryable<T> GetWithOptions(QueryOptions<T> options)
        {
            IQueryable<T> query = context.Set<T>();
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
