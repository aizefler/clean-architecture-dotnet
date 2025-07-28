namespace TodoApp.Core.Common.Events
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in events)
            {
                var handlerType = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());
                var handler = _serviceProvider.GetService(handlerType);
                var method = handlerType.GetMethod("HandleAsync");
                if (method != null)
                {
                    await (Task)method.Invoke(handler, new[] { domainEvent })!;
                }
            }
        }
    }
}
