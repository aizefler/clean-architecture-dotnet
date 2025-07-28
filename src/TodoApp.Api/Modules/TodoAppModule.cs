using TodoApp.Application.TodoItemAggregate.Queries;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.Common;
using TodoApp.Application.TodoItemAggregate.Commands.Dtos;
using TodoApp.Application.TodoItemAggregate.Commands.Interfaces;

namespace TodoApp.Api.Modules
{
    public class TodoAppModule : IRegisterModule
    {
        public void RegisterModule(WebApplication app)
        {
            var sequenciamentoModule = app.MapGroup("/todolistitem")
                .WithTags("ListItem")
                .WithOpenApi();

            sequenciamentoModule.MapGet("", ListarTodoListItems)
                .WithName("ListarTodoItems")
                .WithDescription("Lista todos os items com as suas respectivas listas")
                .WithOpenApi();

            sequenciamentoModule.MapPost("", CriarTodoItem)
                .WithName("CriarTodoItem")
                .WithDescription("Cria um novo TodoItem na lista especificada")
                .WithOpenApi();
        }

        public static async Task<IResult> ListarTodoListItems(
            [FromServices] ITodoQuery todoQuery,
            CancellationToken cancellationToken = default)
        {
            var result = await todoQuery.GetAllAsync();
            return Results.Ok(result);
        }

        public static async Task<IResult> CriarTodoItem(
            [FromBody] TodoItemCreate todoItemCreate,
            [FromServices] ITodoItemCreateService todoItemCreateService,
            CancellationToken cancellationToken = default)
        {
            await todoItemCreateService.CreateAsync(todoItemCreate, cancellationToken);
            return Results.Created($"/todolistitem", null);
        }
    }
}
