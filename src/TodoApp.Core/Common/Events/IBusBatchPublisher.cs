namespace TodoApp.Core.Common.Events
{
    public interface IBusBatchPublisher
    {
        Task SendAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
        Task SendAsync(IEnumerable<MessageDomainEvent> messageDomainEvents, CancellationToken cancellationToken = default);
    }
}