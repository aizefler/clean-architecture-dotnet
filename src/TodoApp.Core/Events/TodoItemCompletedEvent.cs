using TodoApp.Core.Common.Events;
using TodoApp.Core.Entities;

namespace TodoApp.Core.Events;
public class TodoItemCompletedEvent : DomainEvent
{
    public TodoItem TodoItem { get; }

    public TodoItemCompletedEvent(TodoItem todoItem) : base("TodoItem")
    {
        TodoItem = todoItem ?? throw new ArgumentNullException(nameof(todoItem), "Todo item cannot be null");
    }
} 