namespace TodoApp.Application.TodoItemAggregate.Queries.Dtos;

public record TodoListItemDto(
    int IdItem,
    string TitleItem,
    int IdListItem,
    string TitleListItem,
    bool IsCompleted,
    DateTime? CompletedAt);