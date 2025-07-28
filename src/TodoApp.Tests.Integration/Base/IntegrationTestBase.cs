using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using TodoApp.Core.Common.Events;
using TodoApp.Infrastructure.Data.SqlServer;
using TodoApp.Tests.Integration.Setup;

namespace TodoApp.Tests.Integration.Base;

public abstract class IntegrationTestBase : IClassFixture<TodoAppWebApplicationFactory>, IDisposable
{
    protected readonly TodoAppWebApplicationFactory Factory;
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly AppDbContext DbContext;
    protected readonly MockBusPublisher BusPublisher;
    protected readonly MockBusBacthPublisher BusBacthPublisher;

    protected IntegrationTestBase(TodoAppWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        
        // Adiciona headers de autenticação obrigatórios
        Client.DefaultRequestHeaders.Add("x-id-token", CreateValidTestToken());
        
        Scope = factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
        BusPublisher = (MockBusPublisher)Scope.ServiceProvider.GetRequiredService<IBusPublisher>();
        BusBacthPublisher = (MockBusBacthPublisher)Scope.ServiceProvider.GetRequiredService<IBusBacthPublisher>();
        
        // Limpa o banco a cada teste
        CleanDatabase();
    }

    protected virtual void CleanDatabase()
    {
        DbContext.TodoItems.RemoveRange(DbContext.TodoItems);
        DbContext.TodoLists.RemoveRange(DbContext.TodoLists);
        DbContext.MessageDomainEvents.RemoveRange(DbContext.MessageDomainEvents);
        DbContext.SaveChanges();
        
        // Limpa eventos capturados
        BusPublisher.PublishedEvents.Clear();
        BusBacthPublisher.PublishedEvents.Clear();
        BusBacthPublisher.PublishedMessageEvents.Clear();
    }

    protected virtual string CreateValidTestToken()
    {
        // Simula um JWT válido para os testes
        var header = JsonSerializer.Serialize(new { alg = "HS256", typ = "JWT" });
        var payload = JsonSerializer.Serialize(new { 
            preferred_username = "test@example.com",
            exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
        });
        
        var encodedHeader = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(header));
        var encodedPayload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payload));
        
        return $"{encodedHeader}.{encodedPayload}.fake-signature";
    }

    protected async Task<T> PostAsync<T>(string endpoint, object request)
    {
        var response = await Client.PostAsJsonAsync(endpoint, request);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        })!;
    }

    protected async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await Client.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        })!;
    }

    protected void AssertDomainEventWasPublished<TEvent>() where TEvent : class, IDomainEvent
    {
        BusBacthPublisher.PublishedMessageEvents
            .Should().Contain(e => e.Type == typeof(TEvent).FullName);
    }

    protected void AssertDomainEventWasPublished<TEvent>(Func<TEvent, bool> predicate) where TEvent : class, IDomainEvent
    {
        var events = BusBacthPublisher.PublishedMessageEvents
            .Where(e => e.Type == typeof(TEvent).FullName)
            .Select(e => JsonSerializer.Deserialize<TEvent>(e.Content))
            .Where(e => e != null)
            .Cast<TEvent>();

        events.Where(predicate).Should().NotBeEmpty();
    }

    public virtual void Dispose()
    {
        Scope?.Dispose();
        Client?.Dispose();
        GC.SuppressFinalize(this);
    }
}