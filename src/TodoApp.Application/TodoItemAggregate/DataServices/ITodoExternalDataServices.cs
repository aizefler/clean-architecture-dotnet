using TodoApp.Application.TodoItemAggregate.DataServices.Dtos;

namespace TodoApp.Application.TodoItemAggregate.DataServices
{
    public interface ITodoExternalDataServices
    {
        Task<IEnumerable<TodoItemResponse>> GetAllTodoItemsAsync(TodoItemRequest request);
    }
}