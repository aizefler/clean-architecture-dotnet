namespace TodoApp.Application.Common.Dtos
{
    public class Lookup<TKey>
    {
        public TKey Id { get; set; }
        public string Title { get; set; }
    }
}