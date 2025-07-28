using Azure.Messaging.ServiceBus;
using TodoApp.Core.Common.Events;
using System.Text.Json;

namespace TodoApp.Infrastructure.Broker.AzureServiceBus;

public class BusBacthPublisher : IBusBacthPublisher
{

    private readonly ServiceBusClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public BusBacthPublisher(ServiceBusClient client)
    {
        _client = client;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task SendAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {

        if (domainEvents == null || !domainEvents.Any())
            return;

        var topics = domainEvents.GroupBy(x => x.Topic)
                                 .Select(g => g.Key)
                                 .ToList();

        foreach (var topic in topics)
        {

            var _sender = _client.CreateSender(topic);

            ServiceBusMessageBatch batch = await _sender.CreateMessageBatchAsync(cancellationToken);

            foreach (var domainEvent in domainEvents.Where(a => a.Topic == topic))
            {
                var body = JsonSerializer.SerializeToUtf8Bytes(domainEvent, domainEvent.GetType(), _jsonOptions);

                var message = new ServiceBusMessage(body)
                {
                    ContentType = "application/json",
                    Subject = domainEvent.GetType().FullName
                };

                if (!batch.TryAddMessage(message))
                {
                    await _sender.SendMessagesAsync(batch, cancellationToken);
                    batch.Dispose();
                    batch = await _sender.CreateMessageBatchAsync(cancellationToken);

                    if (!batch.TryAddMessage(message))
                        throw new InvalidOperationException("Mensagem muito grande para ser adicionada ao batch do BUS.");
                }
            }

            if (batch.Count > 0)
            {
                await _sender.SendMessagesAsync(batch, cancellationToken);
                batch.Dispose();
            }

            await _sender.DisposeAsync();
        }
    }

    public async Task SendAsync(IEnumerable<MessageDomainEvent> messageDomainEvents, CancellationToken cancellationToken = default)
    {

        if (messageDomainEvents == null || !messageDomainEvents.Any())
            return;

        var topics = messageDomainEvents.GroupBy(x => x.Topic)
                                 .Select(g => g.Key)
                                 .ToList();

        foreach (var topic in topics)
        {

            var _sender = _client.CreateSender(topic);

            ServiceBusMessageBatch batch = await _sender.CreateMessageBatchAsync(cancellationToken);

            foreach (var messageDomainEvent in messageDomainEvents.Where(a => a.Topic == topic))
            {
                var body = JsonSerializer.SerializeToUtf8Bytes(messageDomainEvent, messageDomainEvent.GetType(), _jsonOptions);

                var message = new ServiceBusMessage(body)
                {
                    ContentType = "application/json",
                    Subject = messageDomainEvent.Type
                };

                if (!batch.TryAddMessage(message))
                {
                    await _sender.SendMessagesAsync(batch, cancellationToken);
                    batch.Dispose();
                    batch = await _sender.CreateMessageBatchAsync(cancellationToken);

                    if (!batch.TryAddMessage(message))
                        throw new InvalidOperationException("Mensagem muito grande para ser adicionada ao batch do BUS.");
                }
            }

            if (batch.Count > 0)
            {
                await _sender.SendMessagesAsync(batch, cancellationToken);
                batch.Dispose();
            }

            await _sender.DisposeAsync();
        }
    }

}
