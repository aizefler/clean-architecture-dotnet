using TodoApp.Application.TodoItemAggregate.Queries.Dtos;

namespace TodoApp.Application.TodoItemAggregate.Queries
{
    public interface ITodoQuery
    {
        Task<IEnumerable<TodoListItemDto>> GetAllAsync();
    }
}