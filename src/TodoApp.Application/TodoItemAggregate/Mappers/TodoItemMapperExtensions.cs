using TodoApp.Application.TodoItemAggregate.Commands.Dtos;
using TodoApp.Core.Entities;

namespace TodoApp.Application.TodoItemAggregate.Mappers
{
    public static class TodoItemCreateMapperExtensions
    {
        public static TodoItem ToMap(this TodoItemCreate dto, TodoList? list)
        {
            return new TodoItem(dto.Title, null, PriorityLevel.None, null, list);
        }
    }
}
