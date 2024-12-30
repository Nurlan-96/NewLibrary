using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace NewLibrary.Shared.SeedWork
{
    public interface IRepository<T> where T : BaseEntity
	{
		IUnitOfWork UnitOfWork { get; }
		Task<T> AddAsync(T entity);
        Task<List<T>> GetAllAsync();
		T Update(T entity);
		bool Delete(T entity);
        Task<T> GetWhere(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
		Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    }
}