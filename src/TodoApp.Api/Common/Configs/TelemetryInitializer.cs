using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace TodoApp.Api.Common.Configs;

public class TelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.Cloud.RoleName = "TodoAppApi";
        telemetry.Context.Cloud.RoleInstance = "TodoAppApiInstance";
    }
}
