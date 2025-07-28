# TodoApp.Infrastructure.Broker.AzureServiceBus

Biblioteca responsável pela integração com o Azure Service Bus para publicação de eventos de domínio e mensagens em tópicos.
Centraliza a configuração do cliente Service Bus, publishers para envio individual e em lote, e abstrações para comunicação assíncrona entre sistemas.

## Funcionalidades
- **Publicação de eventos**: Envia eventos de domínio para tópicos do Azure Service Bus.
- **Envio em lote**: Otimiza o envio de múltiplas mensagens, respeitando limites do Service Bus.
- **Serialização automática**: Utiliza JSON para serializar eventos e mensagens.
- **Configuração via DI**: Fácil integração e configuração no projeto principal.

## Estrutura de Componentes

### ServiceCollectionExtensions
Extensão para registrar os publishers e o cliente do Azure Service Bus no container de DI.
```csharp
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
```

### BusPublisher
Publica eventos de domínio individualmente em tópicos do Azure Service Bus.
```csharp
public class BusPublisher : IBusPublisher
{
    public async Task SendAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
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
```

### BusBacthPublisher
Publica múltiplos eventos de domínio ou mensagens em lote, agrupando por tópico.
```csharp
public class BusBacthPublisher : IBusBacthPublisher
{
    public async Task SendAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        // Agrupa por tópico e envia batches
    }
    public async Task SendAsync(IEnumerable<MessageDomainEvent> messageDomainEvents, CancellationToken cancellationToken = default)
    {
        // Agrupa por tópico e envia batches
    }
}
```

---

## Exemplo de Estrutura de Pastas
```
Infrastructure.Broker.AzureServiceBus/
├── ServiceCollectionExtensions.cs
├── BusPublisher.cs
├── BusBacthPublisher.cs
```


