using BaseFaq.Faq.Common.Persistence.FaqDb.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseFaq.Faq.Common.Persistence.FaqDb.Configurations.Base;

public class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasQueryFilter(p => !p.IsDeleted);

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