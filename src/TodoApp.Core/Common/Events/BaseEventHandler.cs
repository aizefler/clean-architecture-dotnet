namespace TodoApp.Core.Common.Events
{
    public abstract class BaseEventHandler<TDomainEvent> : IEventHandler<TDomainEvent>
        where TDomainEvent : DomainEvent
    {

        private readonly IBusPublisher _busPublisher;

        public BaseEventHandler(IBusPublisher busPublisher)
        {
            _busPublisher = busPublisher ?? throw new ArgumentNullException(nameof(busPublisher));
        }

        public async Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            await _busPublisher.SendAsync(domainEvent, cancellationToken);
        }
    }
}
