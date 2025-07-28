using Refit;
using TodoApp.Application.TodoItemAggregate.DataServices;
using TodoApp.Application.TodoItemAggregate.DataServices.Dtos;
using TodoApp.Infrastructure.Data.Services.Common;

namespace TodoApp.Infrastructure.Data.Services.TodoAppExternal
{
    public interface ITodoExternalApi
    {
        [Get("/todos")]
        Task<IEnumerable<TodoItemResponse>> GetAllTodoItemsAsync();
    }

    public class TodoExternalDataServices : BaseApiService, ITodoExternalDataServices
    {
        private readonly ITodoExternalApi _api;

        public TodoExternalDataServices(HttpClient httpClient) : base(httpClient)
        {
            _api = RestService.For<ITodoExternalApi>(httpClient);
        }

        public async Task<IEnumerable<TodoItemResponse>> GetAllTodoItemsAsync()
        {
            return await _api.GetAllTodoItemsAsync();
        }

        public Task<IEnumerable<TodoItemResponse>> GetAllTodoItemsAsync(TodoItemRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
