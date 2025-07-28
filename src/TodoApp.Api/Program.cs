using System.Runtime.CompilerServices;
using TodoApp.Api.Common;
using TodoApp.Api.Common.Configs;
using TodoApp.Api.Common.Middlewares;
using TodoApp.Application;
using TodoApp.Infrastructure.Data.Services;
using TodoApp.Infrastructure.Data.SqlServer;
using TodoApp.Infrastructure.Broker.AzureServiceBus;
using TodoApp.Core.Common;

// Torna a classe Program visível para os testes de integração
[assembly: InternalsVisibleTo("TodoApp.Tests.Integration")]

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.ConfigureAppSettingsCustom();
builder.AddTelemetryConfiguration();
builder.AddHealthCheckConfiguration();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "https://exemplo-origem1.exemplo.com.br",
            "https://exemplo-origem2.exemplo.com.br")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationAggregates();
builder.Services.AddDataServices(builder.Configuration.GetValue<string>("TodoApiBaseUrl"));
builder.Services.AddSqlServerInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddAzureServiceBusBroker(builder.Configuration.GetConnectionString("ServiceBusConnection"));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<AuthHeaderValidationsMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.MapHealthChecks("/health");
app.RegisterModules();
app.Run();

// Torna a classe Program parcial para que possa ser referenciada em testes
public partial class Program { }
