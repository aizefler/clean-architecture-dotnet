namespace TodoApp.Core.Common.Events
{
    public interface IEventHandler<TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
