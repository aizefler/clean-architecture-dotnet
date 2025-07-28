using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TodoApp.Core.Common.Entities;
using TodoApp.Core.Common.Events;

namespace TodoApp.Infrastructure.Data.SqlServer.Common.DomainEvents;

[ExcludeFromCodeCoverage]
public class BusBacthPublisherInterceptor : SaveChangesInterceptor
{
    private readonly IBusBacthPublisher _busBacthPublisher;

    public BusBacthPublisherInterceptor(IBusBacthPublisher busBacthPublisher)
    {
        _busBacthPublisher = busBacthPublisher;
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        PublishMessagesAsync(eventData.Context).GetAwaiter().GetResult();
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            await PublishMessagesAsync(eventData.Context, cancellationToken);
        }
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task PublishMessagesAsync(DbContext? context, CancellationToken cancellationToken = default)
    {
        if (context == null) return;

        var messages = context.ChangeTracker
            .Entries<MessageDomainEvent>()
            .Where(e => e.State == EntityState.Unchanged)
            .Select(e => e.Entity)
            .ToList();

        await _busBacthPublisher.SendAsync(messages.ToArray(), cancellationToken);
    }
}