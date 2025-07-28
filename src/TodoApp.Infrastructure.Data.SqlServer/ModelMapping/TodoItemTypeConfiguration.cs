using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using TodoApp.Core.Entities;

namespace TodoApp.Infrastructure.Data.SqlServer.ModelMapping;

[ExcludeFromCodeCoverage]
public class TodoItemTypeConfiguration : BaseAuditableEntityTypeConfiguration<TodoItem>
{
    public override void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("TodoItems");

        // Title configuration
        builder.Property(e => e.Title)
            .HasMaxLength(255);

        // Note configuration
        builder.Property(e => e.Note)
            .HasColumnType("nvarchar(max)");

        // Priority configuration - enum stored as int
        builder.Property(e => e.Priority)
            .IsRequired()
            .HasConversion<int>();

        // Reminder configuration
        builder.Property(e => e.Reminder)
            .HasColumnType("datetime2(7)");

        // Done configuration with backing field
        builder.Property(e => e.Done)
            .IsRequired()
            .HasDefaultValue(false);

        // Foreign key configuration
        builder.HasOne(e => e.List)
            .WithMany(e => e.Items)
            .HasForeignKey("ListId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(e => e.Title);
        builder.HasIndex(e => e.Done);
        builder.HasIndex(e => e.Priority);
        builder.HasIndex("ListId");
    }
} 