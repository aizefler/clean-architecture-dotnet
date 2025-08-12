using TodoApp.Core.Common.Events;
using TodoApp.Core.Events;

namespace TodoApp.Application.TodoItemAggregate.EventHandlers;

public class TodoItemCreatedEventHandler(IBusPublisher busPublisher) : BaseEventHandler<TodoItemCreatedEvent>(busPublisher);
