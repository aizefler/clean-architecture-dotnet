using TodoApp.Core.Common.Events;
using TodoApp.Core.Events;

namespace TodoApp.Application.TodoItemAggregate.EventHandlers
{
    public class TodoItemCompletedEventHandler : BaseEventHandler<TodoItemCompletedEvent>
    {
        public TodoItemCompletedEventHandler(IBusPublisher busPublisher) : base(busPublisher) { }
    }
}
