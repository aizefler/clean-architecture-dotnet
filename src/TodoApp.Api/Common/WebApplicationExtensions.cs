using System.Diagnostics.CodeAnalysis;

namespace TodoApp.Api.Common;

[ExcludeFromCodeCoverage]
public static class WebApplicationExtensions
{
    public static WebApplicationBuilder ConfigureAppSettingsCustom(this WebApplicationBuilder builder)
    {
        var appSettingsFolder = Environment.GetEnvironmentVariable("APPSETTINGS_DIR");

        if (string.IsNullOrWhiteSpace(appSettingsFolder))
            appSettingsFolder = ".";

        builder.Configuration
            .AddJsonFile($"{appSettingsFolder}/appsettings.json", reloadOnChange: true, optional: true)
            .AddJsonFile($"{appSettingsFolder}/appsettings.{builder.Environment.EnvironmentName}.json", reloadOnChange: true, optional: true)
            .AddEnvironmentVariables();

        return builder;
    }
}
