using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using TodoApp.Core.Common.Data;
using TodoApp.Core.Common.Entities;

namespace TodoApp.Infrastructure.Data.SqlServer.Common.Repositories;

[ExcludeFromCodeCoverage]
public class Repository<T> : IRepository<T> where T : BaseEntity<int>
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync<TKey>(TKey id)
    {
        // Note: This method signature in IRepository uses Guid but our entities use int
        // This might need to be adjusted in the interface or we need to handle conversion
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IReadOnlyList<T>> ListAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task<IReadOnlyList<T>> ListAsync<TKey>(
        Expression<Func<T, bool>>? predicate,
        Expression<Func<T, TKey>>? orderBy,
        bool ascending = true,
        int skip = 0,
        int take = 10)
    {
        IQueryable<T> query = _dbSet;

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
        }

        return await query.Skip(skip).Take(take).ToListAsync();
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        return predicate == null 
            ? await _dbSet.CountAsync() 
            : await _dbSet.CountAsync(predicate);
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        var result = await _dbSet.AddAsync(entity);
        return result.Entity;
    }

    public virtual Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity)
    {
        // Soft delete
        entity.Deleted = true;
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }
} 