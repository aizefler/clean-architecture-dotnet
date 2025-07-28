namespace TodoApp.Application.TodoItemAggregate.Queries.Dtos
{
    public class TodoListItemDto
    {
        public int IdItem { get; set; }
        public string TitleItem { get; set; }
        public int IdListItem { get; set; }
        public string TitleListItem { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}