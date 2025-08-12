using TodoApp.Application.TodoItemAggregate.Queries.Dtos;

namespace TodoApp.Application.TodoItemAggregate.Queries.Mappers;

public static class TodoListItemMapper
{
    public static IEnumerable<TodoListItemCountDto> ToMap(IEnumerable<TodoListItemDto> todolistItems)
    {
        var itemsGroup = todolistItems
            .GroupBy(s => s.IdListItem)
            .Select(s => new TodoListItemCountDto(
                s.First().IdItem,
                s.First().TitleListItem,
                s.Any(i => i.IsCompleted),
                s.FirstOrDefault(i => i.CompletedAt.HasValue)?.CompletedAt,
                s.Count()));

        return itemsGroup;
    }
}