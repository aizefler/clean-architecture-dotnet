using Azure.Messaging.ServiceBus;
using TodoApp.Core.Common.Events;
using System.Text.Json;

namespace TodoApp.Infrastructure.Broker.AzureServiceBus;

public class BusPublisher : IBusPublisher
{
    private readonly ServiceBusClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public BusPublisher(ServiceBusClient client)
    {
        _client = client;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task SendAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {

        if (string.IsNullOrWhiteSpace(domainEvent?.Topic))
        {
            throw new ArgumentException("Domain event topic cannot be null or empty.", nameof(domainEvent));
        }

        var sender = _client.CreateSender(domainEvent.Topic);
        var body = JsonSerializer.SerializeToUtf8Bytes(domainEvent, domainEvent.GetType(), _jsonOptions);
        var message = new ServiceBusMessage(body)
        {
            ContentType = "application/json",
            Subject = domainEvent.GetType().FullName
        };
        await sender.SendMessageAsync(message, cancellationToken);
    }
}
