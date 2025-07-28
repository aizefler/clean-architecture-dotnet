using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using TodoApp.Core.Common.Data;
using TodoApp.Core.Common.Entities;

namespace TodoApp.Infrastructure.Data.SqlServer.Common.Repositories;

[ExcludeFromCodeCoverage]
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private bool _disposed = false;

    public UnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        
        return (IRepository<T>)_repositories.GetOrAdd(type, _ =>
        {
            if (typeof(BaseEntity<int>).IsAssignableFrom(type))
            {
                var repositoryType = typeof(Repository<>).MakeGenericType(type);
                return Activator.CreateInstance(repositoryType, _context)!;
            }
            
            throw new InvalidOperationException($"Type {type.Name} is not supported by this repository implementation.");
        });
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context?.Dispose();
                _repositories.Clear();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
} 