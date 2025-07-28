using Azure.Messaging.ServiceBus;
using TodoApp.Core.Common.Events;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace TodoApp.Infrastructure.Broker.AzureServiceBus;

public class BusBacthPublisher : IBusBacthPublisher
{

    private readonly ServiceBusClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<BusBacthPublisher> _logger;

    public BusBacthPublisher(ServiceBusClient client, 
        ILogger<BusBacthPublisher> logger)
    {
        _logger = logger;
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
        {
            _logger.LogWarning("Nenhum evento de domínio para enviar.");
            return;
        }

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

        await SendAsync(messageDomainEvents, cancellationToken);
    }

    public async Task SendAsync(IEnumerable<MessageDomainEvent> messageDomainEvents, CancellationToken cancellationToken = default)
    {

        if (messageDomainEvents == null || !messageDomainEvents.Any())
        {
            _logger.LogWarning("Nenhum evento de domínio para enviar.");
            return;
        }

        var topics = messageDomainEvents.GroupBy(x => x.Topic)
                                 .Select(g => g.Key)
                                 .ToList();

        foreach (var topic in topics)
        {
            if (string.IsNullOrEmpty(topic))
            {
                _logger.LogWarning("Tópico vazio ou nulo encontrado. Evento não será enviado.");
                continue;
            }

            var _sender = _client.CreateSender(topic);

            ServiceBusMessageBatch batch = await _sender.CreateMessageBatchAsync(cancellationToken);

            var eventsCount = messageDomainEvents.Count(a => a.Topic == topic);
            _logger.LogInformation("Enviando {eventsCount} eventos de domínio para o tópico {Topic}", eventsCount, topic);

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
