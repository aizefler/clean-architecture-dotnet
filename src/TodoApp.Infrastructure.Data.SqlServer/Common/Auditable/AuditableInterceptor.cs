using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TodoApp.Core.Common;
using TodoApp.Core.Common.Entities;

namespace TodoApp.Infrastructure.Data.SqlServer.Common.Auditable;

[ExcludeFromCodeCoverage]
public class AuditableInterceptor : SaveChangesInterceptor
{
    private readonly IUserContext? _userContext;

    public AuditableInterceptor(IUserContext? userContext = null)
    {
        _userContext = userContext;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            SetAuditFields(eventData.Context);
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
            SetAuditFields(eventData.Context);
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetAuditFields(DbContext context)
    {
        var now = DateTimeOffset.UtcNow;
        var user = !string.IsNullOrEmpty(_userContext?.UserName) ? _userContext.UserName : "Unknown";

        var auditableEntities = context.ChangeTracker.Entries<BaseAuditableEntity<int>>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entityEntry in auditableEntities)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Entity.CreatedAt = now;
                entityEntry.Entity.CreatedBy ??= user;
            }
            entityEntry.Entity.UpdatedAt = now;
            entityEntry.Entity.UpdatedBy = user;
        }
    }
}