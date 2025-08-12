using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using TodoApp.Application.TodoItemAggregate.Queries;
using TodoApp.Core.Common.Data;
using TodoApp.Core.Common.Events;
using TodoApp.Infrastructure.Data.SqlServer.Common.Auditable;
using TodoApp.Infrastructure.Data.SqlServer.Common.DomainEvents;
using TodoApp.Infrastructure.Data.SqlServer.Common.Repositories;
using TodoApp.Infrastructure.Data.SqlServer.Queries;

namespace TodoApp.Infrastructure.Data.SqlServer;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlServerInfrastructure(
        this IServiceCollection services, 
        string connectionString)
    {
        // Add services
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<DomainEventInterceptor>();
        services.AddScoped<AuditableInterceptor>();
        services.AddScoped<BusBatchPublisherInterceptor>();

        // Add query repositories
        services.AddScoped<ITodoQuery, TodoQueryRepository>();

        // Add DbContext
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {

            options.UseSqlServer(connectionString, builder =>
            {
                builder.EnableRetryOnFailure(maxRetryCount: 3);
            });

            options.AddInterceptors(
                serviceProvider.GetRequiredService<DomainEventInterceptor>(),
                serviceProvider.GetRequiredService<AuditableInterceptor>(),
                serviceProvider.GetRequiredService<BusBatchPublisherInterceptor>()
            );

            // Enable sensitive data logging in development
#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });

        // Add repositories and unit of work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
} 