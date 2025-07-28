namespace TodoApp.Application.TodoItemAggregate.Queries.Dtos
{
    public class TodoListItemCountDto
    {
        public int IdItem { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int CountItems { get; set; }
    }
}