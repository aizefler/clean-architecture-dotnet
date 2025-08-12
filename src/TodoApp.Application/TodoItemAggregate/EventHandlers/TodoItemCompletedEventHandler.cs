using TodoApp.Core.Common.Events;
using TodoApp.Core.Events;

namespace TodoApp.Application.TodoItemAggregate.EventHandlers;

public class TodoItemCompletedEventHandler(IBusPublisher busPublisher) : BaseEventHandler<TodoItemCompletedEvent>(busPublisher);
