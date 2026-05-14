using System.Linq.Expressions;

namespace FSP.Api.Domain.Interfaces.Data.Repositories
{
    public interface IRepositoryBase<T> where T : class
    {
        T Add(T evento);
        T Update(T evento);
        IEnumerable<T> UpdateRange(IEnumerable<T> evento);
        void Delete(T evento);
        void DeleteRange(T[] evento);
        IEnumerable<T> GetAll();
        T? Get(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetRanged(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetWithIncludes(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] including);
        IQueryable<T> AsQueryable();
    }
}
