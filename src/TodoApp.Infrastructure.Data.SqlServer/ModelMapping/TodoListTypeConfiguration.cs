using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using TodoApp.Core.Entities;

namespace TodoApp.Infrastructure.Data.SqlServer.ModelMapping;

[ExcludeFromCodeCoverage]
public class TodoListTypeConfiguration : BaseAuditableEntityTypeConfiguration<TodoList>
{
    public override void Configure(EntityTypeBuilder<TodoList> builder)
    {
        base.Configure(builder);

        builder.ToTable("TodoLists");

        // Title configuration
        builder.Property(e => e.Title)
            .HasMaxLength(255);

        // Color configuration - storing as ARGB integer
        builder.Property(e => e.Colour)
            .IsRequired()
            .HasConversion(
                color => color.ToArgb(),
                argb => System.Drawing.Color.FromArgb(argb)
            )
            .HasColumnName("Colour");

        // Navigation property configuration
        builder.HasMany(e => e.Items)
            .WithOne(e => e.List)
            .HasForeignKey("ListId")
            .OnDelete(DeleteBehavior.Cascade);

        // Index for performance
        builder.HasIndex(e => e.Title);
    }
} 