using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Core.Common.Events;

namespace TodoApp.Infrastructure.Data.SqlServer.Common.DomainEvents
{
    public class MessageDomainEventTypeConfiguration : IEntityTypeConfiguration<MessageDomainEvent>
    {
        public void Configure(EntityTypeBuilder<MessageDomainEvent> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Type)
                .HasColumnType("nvarchar(255)");

            builder.Property(e => e.Content)
                .HasColumnType("nvarchar(max)");
            
            builder.Property(e => e.Topic)
                .HasColumnType("nvarchar(255)");

            builder.Property(e => e.OccurredOn)
                .HasColumnType("datetimeoffset(7)")
                .IsRequired();

            builder.Property(e => e.ProcessedOn)
                .HasColumnType("datetimeoffset(7)");

            builder.Property(e => e.Processed)
                .IsRequired()
                .HasDefaultValue(false);

        }
    }
}
