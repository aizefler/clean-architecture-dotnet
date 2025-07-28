using TodoApp.Core.Common;
using TodoApp.Core.Common.Events;

namespace TodoApp.Tests.Integration.Setup;

public class MockUserContext : IUserContext
{
    public string Id => "test-user-id";
    public string UserName => "test-user";
    public string Email => "test@example.com";

    public string GetTokenBearer() => "mock-bearer-token";
    public string GetXIdToken() => "mock-id-token";
}

public class MockBusPublisher : IBusPublisher
{
    public List<DomainEvent> PublishedEvents { get; } = new();

    public Task SendAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        PublishedEvents.Add(domainEvent);
        return Task.CompletedTask;
    }
}

public class MockBusBacthPublisher : IBusBacthPublisher
{
    public List<IDomainEvent> PublishedEvents { get; } = new();
    public List<MessageDomainEvent> PublishedMessageEvents { get; } = new();

    public Task SendAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        PublishedEvents.AddRange(domainEvents);
        return Task.CompletedTask;
    }

    public Task SendAsync(IEnumerable<MessageDomainEvent> messageDomainEvents, CancellationToken cancellationToken = default)
    {
        PublishedMessageEvents.AddRange(messageDomainEvents);
        return Task.CompletedTask;
    }
}