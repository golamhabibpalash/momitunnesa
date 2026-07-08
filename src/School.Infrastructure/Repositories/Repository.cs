using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using School.Domain.Common;
using School.Domain.Interfaces;
using School.Infrastructure.Data;

namespace School.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }

    public virtual T Update(T entity)
    {
        var entry = Context.Entry(entity);
        entry.State = EntityState.Modified;
        return entity;
    }

    public virtual void Delete(T entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        var entry = Context.Entry(entity);
        entry.State = EntityState.Modified;
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        return await Context.SaveChangesAsync();
    }

    public virtual IQueryable<T> GetAllAsQueryable()
    {
        return DbSet.AsNoTracking().AsQueryable();
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = DbSet.AsNoTracking();
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public virtual async Task<int> CountAsync()
    {
        return await DbSet.CountAsync();
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.AnyAsync(predicate);
    }

    public virtual async Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await DbSet.AsNoTracking().Where(e => ids.Contains(e.Id)).ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetActiveAsync()
    {
        var isActiveProperty = typeof(T).GetProperty("IsActive");
        if (isActiveProperty != null)
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, isActiveProperty);
            var trueConstant = Expression.Constant(true);
            var condition = Expression.Equal(property, trueConstant);
            var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);

            return await DbSet.AsNoTracking().Where(lambda).ToListAsync();
        }

        var statusProperty = typeof(T).GetProperty("Status");
        if (statusProperty != null)
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, statusProperty);
            var trueConstant = Expression.Constant(true);
            var condition = Expression.Equal(property, trueConstant);
            var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);

            return await DbSet.AsNoTracking().Where(lambda).ToListAsync();
        }

        return await DbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetInactiveAsync()
    {
        var isActiveProperty = typeof(T).GetProperty("IsActive");
        if (isActiveProperty != null)
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, isActiveProperty);
            var falseConstant = Expression.Constant(false);
            var condition = Expression.Equal(property, falseConstant);
            var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);

            return await DbSet.AsNoTracking().Where(lambda).ToListAsync();
        }

        var statusProperty = typeof(T).GetProperty("Status");
        if (statusProperty != null)
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, statusProperty);
            var falseConstant = Expression.Constant(false);
            var condition = Expression.Equal(property, falseConstant);
            var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);

            return await DbSet.AsNoTracking().Where(lambda).ToListAsync();
        }

        return await DbSet.AsNoTracking().ToListAsync();
    }
}
