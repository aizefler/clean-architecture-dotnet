using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Application.Common;
using TodoApp.Application.TodoItemAggregate.Commands;
using TodoApp.Application.TodoItemAggregate.Commands.Interfaces;
using TodoApp.Application.TodoItemAggregate.Validations;
using TodoApp.Core.Common;
using TodoApp.Core.Entities;

namespace TodoApp.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationAggregates(this IServiceCollection services)
        {
            services.AddScoped<ITodoItemCreateService, TodoItemCreateService>();
            services.AddScoped<IValidator<TodoItem>, TodoItemValidator>();
            services.AddScoped<IUserRolesContext, UserRolesContext>();

            return services;
        }
    }
}