using TodoApp.Core.Common.Entities;
using TodoApp.Core.Events;

namespace TodoApp.Core.Entities;

public class TodoItem : BaseAuditableEntity<int>
{
    public string? Title { get; set; }

    public string? Note { get; set; }

    public PriorityLevel Priority { get; set; }

    public DateTime? Reminder { get; set; }

    private bool _done;
    public bool Done
    {
        get => _done;
        set
        {
            if (value && !_done)
            {
                AddDomainEvent(new TodoItemCompletedEvent(this));
            }

            _done = value;
        }
    }

    public TodoList List { get; set; } = null!;

    protected TodoItem() { }

    public TodoItem(string? title, string? note, PriorityLevel priority, DateTime? reminder, TodoList? list)
    {
        Title = title;
        Note = note;
        Priority = priority;
        Reminder = reminder;
        List = list;
    }
}
