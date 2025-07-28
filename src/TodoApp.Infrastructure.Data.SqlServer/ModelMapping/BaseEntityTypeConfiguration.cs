using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using TodoApp.Core.Common.Entities;

namespace TodoApp.Infrastructure.Data.SqlServer.ModelMapping;

[ExcludeFromCodeCoverage]
public abstract class BaseEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity<int>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Primary key configuration
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Deleted flag configuration
        builder.Property(e => e.Deleted)
            .IsRequired()
            .HasDefaultValue(false);
            
        // Query filter to exclude deleted records
        builder.HasQueryFilter(e => !e.Deleted);
    }
}

[ExcludeFromCodeCoverage]
public abstract class BaseAuditableEntityTypeConfiguration<TEntity> : BaseEntityTypeConfiguration<TEntity> where TEntity : BaseAuditableEntity<int>
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        // Standard audit fields configuration
        builder.Property(e => e.CreatedAt)
            .HasColumnType("datetimeoffset(7)")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("datetimeoffset(7)");

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(255);
    }
}
