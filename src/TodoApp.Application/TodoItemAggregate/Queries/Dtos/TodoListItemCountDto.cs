namespace TodoApp.Application.TodoItemAggregate.Queries.Dtos;

public record TodoListItemCountDto(
    int IdItem,
    string Title,
    bool IsCompleted,
    DateTime? CompletedAt,
    int CountItems);