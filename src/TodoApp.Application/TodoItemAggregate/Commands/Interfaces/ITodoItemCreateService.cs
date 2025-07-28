using TodoApp.Application.TodoItemAggregate.Commands.Dtos;

namespace TodoApp.Application.TodoItemAggregate.Commands.Interfaces
{
    public interface ITodoItemCreateService
    {
        Task CreateAsync(TodoItemCreate todoItem, CancellationToken cancellationToken = default);
    }
}