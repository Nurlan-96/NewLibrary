using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NewLibrary.Shared.Exceptions;
using NewLibrary.Shared.SeedWork;
using System.Linq.Expressions;


namespace NewLibrary.Infrastructure.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : BaseEntity
    {
        public abstract DbContext Context { get; protected set; }
        public DbSet<T> Table => Context.Set<T>();
        public IUnitOfWork UnitOfWork => Context as IUnitOfWork;

        public async Task<T> AddAsync(T entity)
        {
            var entry = await Context.Set<T>().AddAsync(entity);
            return entry.Entity;
        }

        public bool Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
            return true;
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = Context.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }

        public Task<List<T>> GetAllAsync()
        {
            return Table.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = Context.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate)
                ?? throw new EntityNotFoundException<T>();
        }

        public async Task<T> GetWhere(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = Table.Where(predicate);

            if (include != null)
            {
                query = include(query);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }

        public T Update(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await Table.AnyAsync(predicate);
        }
    }
}