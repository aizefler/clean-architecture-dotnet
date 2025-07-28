using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Core.Common.Events;

namespace TodoApp.Infrastructure.Broker.AzureServiceBus
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureServiceBusBroker(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton(new ServiceBusClient(connectionString));
            services.AddScoped<IBusPublisher, BusPublisher>();
            services.AddScoped<IBusBacthPublisher, BusBacthPublisher>();
            return services;
        }
    }
}
