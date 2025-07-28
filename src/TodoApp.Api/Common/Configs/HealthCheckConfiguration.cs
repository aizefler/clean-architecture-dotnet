using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TodoApp.Api.Common.Configs;

public static class HealthCheckConfiguration
{
    public static IServiceCollection AddHealthCheckConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("API is running."),
                tags: new[] { "self" });

        return builder.Services;
    }
}
