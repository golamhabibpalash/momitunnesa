using System.Linq.Expressions;
using School.Domain.Common;

namespace School.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);

    Task<IEnumerable<T>> GetAllAsync();

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);

    T Update(T entity);

    void Delete(T entity);

    Task<int> SaveChangesAsync();

    IQueryable<T> GetAllAsQueryable();

    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);

    Task<int> CountAsync();

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids);

    Task<IEnumerable<T>> GetActiveAsync();

    Task<IEnumerable<T>> GetInactiveAsync();
}
