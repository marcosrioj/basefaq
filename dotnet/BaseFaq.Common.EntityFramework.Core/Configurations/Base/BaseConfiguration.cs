using BaseFaq.Common.EntityFramework.Core.Entities;
using BaseFaq.Common.EntityFramework.Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseFaq.Common.EntityFramework.Core.Configurations.Base;

public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasIndex(p => p.IsDeleted)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_IsDeleted");

        builder.Property(c => c.IsDeleted);

        builder.Property(c => c.DeletedBy)
            .IsRequired(false);

        builder.Property(c => c.DeletedDate)
            .IsRequired(false);

        builder.Property(c => c.CreatedBy)
            .IsRequired(false);

        builder.Property(c => c.CreatedDate)
            .IsRequired(false);

        builder.Property(c => c.UpdatedBy)
            .IsRequired(false);

        builder.Property(c => c.UpdatedDate)
            .IsRequired(false);
    }
}