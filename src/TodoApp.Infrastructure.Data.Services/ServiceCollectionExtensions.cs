using Microsoft.Extensions.DependencyInjection;
using TodoApp.Application.TodoItemAggregate.DataServices;
using TodoApp.Infrastructure.Data.Services.Common;
using TodoApp.Infrastructure.Data.Services.TodoAppExternal;
using Refit;

namespace TodoApp.Infrastructure.Data.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, string todoApiBaseUrl)
        {
            services.AddScoped<ITodoExternalDataServices, TodoExternalDataServices>();
            services.AddScoped<AuthHeaderHandler>();

            services.AddRefitClient<ITodoExternalApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(todoApiBaseUrl))
                .AddHttpMessageHandler<AuthHeaderHandler>();
            return services;
        }
    }
}
