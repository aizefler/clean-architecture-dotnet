namespace TodoApp.Core.Common.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
