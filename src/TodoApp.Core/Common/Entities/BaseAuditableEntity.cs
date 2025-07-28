namespace TodoApp.Core.Common.Entities
{
    public class BaseAuditableEntity<TKey> : BaseEntity<TKey>
    {
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string? UpdatedBy { get; set; }
    }
}
