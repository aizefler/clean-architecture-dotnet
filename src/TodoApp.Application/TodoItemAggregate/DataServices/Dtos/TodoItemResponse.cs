namespace TodoApp.Application.TodoItemAggregate.DataServices.Dtos
{
    public class TodoItemResponse
    {
        public int Id { get; set; }
        public int ListId { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}