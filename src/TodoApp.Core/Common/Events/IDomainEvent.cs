namespace TodoApp.Core.Common.Events
{
    public interface IDomainEvent
    {
        Guid Id { get; }
        DateTime OccurredOn { get; }
        string Topic { get; }
    }
}
