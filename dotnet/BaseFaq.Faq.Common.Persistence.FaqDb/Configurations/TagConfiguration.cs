using BaseFaq.Common.EntityFramework.Core.Configurations;
using BaseFaq.Faq.Common.Persistence.FaqDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseFaq.Faq.Common.Persistence.FaqDb.Configurations;

public class TagConfiguration : BaseConfiguration<Tag>
{
    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        base.Configure(builder);

        builder.ToTable("Tags");

        builder.Property(p => p.Value)
            .HasMaxLength(Tag.MaxValueLength)
            .IsRequired();

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.HasIndex(p => p.Value)
            .HasDatabaseName("IX_Tag_Value");
    }
}