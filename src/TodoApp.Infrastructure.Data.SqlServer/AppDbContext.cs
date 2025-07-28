using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TodoApp.Core.Common.Events;
using TodoApp.Core.Entities;
using TodoApp.Infrastructure.Data.SqlServer.Common.Auditable;
using TodoApp.Infrastructure.Data.SqlServer.Common.DomainEvents;

namespace TodoApp.Infrastructure.Data.SqlServer;

[ExcludeFromCodeCoverage]
public class AppDbContext : DbContext
{
    private readonly DomainEventInterceptor _domainEventInterceptor;
    private readonly AuditableInterceptor _auditableInterceptor;

    public AppDbContext(DbContextOptions<AppDbContext> options, DomainEventInterceptor domainEventInterceptor, AuditableInterceptor auditableInterceptor) 
        : base(options)
    {
        _domainEventInterceptor = domainEventInterceptor;
        _auditableInterceptor = auditableInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_domainEventInterceptor);
        optionsBuilder.AddInterceptors(_auditableInterceptor);
    }

    public DbSet<TodoList> TodoLists => Set<TodoList>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<MessageDomainEvent> MessageDomainEvents => Set<MessageDomainEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        return base.SaveChanges();
    }
} 