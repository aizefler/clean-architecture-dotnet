using System.Text.Json;

namespace TodoApp.Core.Common.Events
{
    public class DomainEvent : IDomainEvent
    {
        public Guid Id { get; private set; }
        public DateTime OccurredOn { get; private set; }
        public string Topic { get; private set; }
        public DomainEvent(string topic)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            Topic = topic;
        }
    }
}
