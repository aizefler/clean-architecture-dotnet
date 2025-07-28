using System.Linq.Expressions;

namespace TodoApp.Core.Common.Data
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync<Tkey>(Tkey id);
        Task<IReadOnlyList<T>> ListAsync();
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate);
        Task<IReadOnlyList<T>> ListAsync<TKey>(
            Expression<Func<T, bool>>? predicate,
            Expression<Func<T, TKey>>? orderBy,
            bool ascending = true,
            int skip = 0,
            int take = 10);

        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
