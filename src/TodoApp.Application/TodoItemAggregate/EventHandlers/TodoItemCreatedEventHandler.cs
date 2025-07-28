using TodoApp.Core.Common.Events;
using TodoApp.Core.Events;

namespace TodoApp.Application.TodoItemAggregate.EventHandlers
{
    public class TodoItemCreatedEventHandler : BaseEventHandler<TodoItemCreatedEvent>
    {
        public TodoItemCreatedEventHandler(IBusPublisher busPublisher) : base(busPublisher)
        {
        }
    }
}
