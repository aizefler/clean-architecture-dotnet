using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TodoApp.Core.Common.Entities;
using TodoApp.Core.Common.Events;
using System.Text.Json;

namespace TodoApp.Infrastructure.Data.SqlServer.Common.DomainEvents;

[ExcludeFromCodeCoverage]
public class DomainEventInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            SaveDomainEvents(eventData.Context);
        }
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            SaveDomainEvents(eventData.Context);
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SaveDomainEvents(DbContext context)
    {

        var domainEvents = context.ChangeTracker
            .Entries<BaseEntity<int>>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        if (domainEvents is null || domainEvents.Count == 0) return;

        var messageDomainEvents = new List<MessageDomainEvent>();

        foreach (var domainEvent in domainEvents)
        {
            var messageDomainEvent = new MessageDomainEvent
            {
                Id = domainEvent.Id,
                OccurredOn = domainEvent.OccurredOn,
                Topic = domainEvent.Topic,
                Type = domainEvent.GetType().FullName ?? "Unknown",
                Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), new JsonSerializerOptions { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve }),
                Processed = false,
            };

            messageDomainEvents.Add(messageDomainEvent);
        }

        context.AddRange(messageDomainEvents);

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity<int>>())
        {
            entry.Entity.ClearDomainEvents();
        }
    }
}