using Microsoft.Extensions.Logging.ApplicationInsights;
using System.Diagnostics.CodeAnalysis;

namespace TodoApp.Api.Common.Configs;

[ExcludeFromCodeCoverage]
public static class TelemetryConfiguration
{
    public static IServiceCollection AddTelemetryConfiguration(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsEnvironment("IntegrationTests") || builder.Environment.IsDevelopment())
            return builder.Services;

        string telemetryConnectionString = builder.Configuration["ApplicationInsights:InstrumentationKey"]!;

        builder.Services.AddApplicationInsightsTelemetry((options) =>
        {
            options.ConnectionString = telemetryConnectionString;
        });

        builder.Logging.AddApplicationInsights(
            configureTelemetryConfiguration: (config) =>
                config.ConnectionString = telemetryConnectionString,
            configureApplicationInsightsLoggerOptions: (options) => { })
        .AddFilter<ApplicationInsightsLoggerProvider>(typeof(Program).FullName, LogLevel.Trace);

        return builder.Services;
    }
}
