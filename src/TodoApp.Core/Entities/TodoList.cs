using System.Drawing;
using TodoApp.Core.Common.Entities;

namespace TodoApp.Core.Entities;

public class TodoList : BaseAuditableEntity<int>
{
    public string? Title { get; set; }

    public Color Colour { get; set; } = Color.White;

    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
}
