using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using TodoApp.Core.Common;
using TodoApp.Core.Common.Events;
using TodoApp.Infrastructure.Data.SqlServer;
using WireMock.Server;

namespace TodoApp.Tests.Integration.Setup;

public class TodoAppWebApplicationFactory : WebApplicationFactory<Program>
{
    private WireMockServer? _wireMockServer;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext real
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.RemoveAll(typeof(AppDbContext));

            // Adiciona DbContext InMemory para testes rápidos
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
                options.EnableSensitiveDataLogging();
            });

            // Remove serviços externos
            services.RemoveAll<IBusBacthPublisher>();
            services.RemoveAll<IBusPublisher>();

            // Mock para serviços externos
            services.AddScoped<IBusBacthPublisher, MockBusBacthPublisher>();
            services.AddScoped<IBusPublisher, MockBusPublisher>();

            // Mock para IUserContext
            services.RemoveAll<IUserContext>();
            services.AddScoped<IUserContext, MockUserContext>();

            // Configure WireMock para APIs externas
            SetupWireMockServer();
            
            // Substitui a URL da API externa pela do WireMock
            services.Configure<ExternalApiSettings>(options =>
            {
                options.TodoApiBaseUrl = _wireMockServer!.Urls[0];
            });
        });

        builder.UseEnvironment("Testing");
    }

    private void SetupWireMockServer()
    {
        _wireMockServer = WireMockServer.Start();
        
        // Configure mock responses para APIs externas
        _wireMockServer
            .Given(WireMock.RequestBuilders.Request.Create()
                .WithPath("/todos")
                .UsingGet())
            .RespondWith(WireMock.ResponseBuilders.Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""
                [
                    {
                        "id": 1,
                        "title": "External Todo Item",
                        "completed": false
                    }
                ]
                """));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _wireMockServer?.Stop();
            _wireMockServer?.Dispose();
        }
        base.Dispose(disposing);
    }
}

public class ExternalApiSettings
{
    public string TodoApiBaseUrl { get; set; } = string.Empty;
}