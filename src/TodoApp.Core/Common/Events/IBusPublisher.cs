namespace TodoApp.Core.Common.Events
{
    public interface IBusPublisher
    {
        Task SendAsync(DomainEvent domainEvents, CancellationToken cancellationToken = default);
    }
}
