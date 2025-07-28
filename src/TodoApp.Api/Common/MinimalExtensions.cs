using System.Diagnostics.CodeAnalysis;

namespace TodoApp.Api.Common;

[ExcludeFromCodeCoverage]
public static class MinimalExtensions
{
    public static void RegisterModules(this WebApplication app)
    {
        var moduleDefinitions = typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IRegisterModule)) && !t.IsAbstract && !t.IsInterface)
            .Select(Activator.CreateInstance)
            .Cast<IRegisterModule>();

        foreach (var module in moduleDefinitions)
        {
            module.RegisterModule(app);
        }
    }
}
