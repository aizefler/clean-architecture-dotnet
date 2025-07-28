using TodoApp.Application.TodoItemAggregate.Queries.Dtos;

namespace TodoApp.Application.TodoItemAggregate.Queries.Mappers
{
    public static class TodoListItemMapper
    {
        public static IEnumerable<TodoListItemCountDto> ToMap(IEnumerable<TodoListItemDto> todolistItems)
        {
            var itemsGroup = todolistItems
                .GroupBy(s => s.IdListItem)
                .Select((s)=> { 
                    return new TodoListItemCountDto
                    {
                        IdItem = s.First().IdItem,
                        Title = s.First().TitleListItem,
                        IsCompleted = s.Any(i => i.IsCompleted),
                        CompletedAt = s.FirstOrDefault(i => i.CompletedAt.HasValue)?.CompletedAt,
                        CountItems = s.Count()
                    };
                });

            return itemsGroup;
        }
    }
}