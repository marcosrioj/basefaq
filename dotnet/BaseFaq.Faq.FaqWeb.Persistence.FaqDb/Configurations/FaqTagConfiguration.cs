using BaseFaq.Common.EntityFramework.Core.Configurations;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Configurations;

public class FaqTagConfiguration : BaseConfiguration<FaqTag>
{
    public override void Configure(EntityTypeBuilder<FaqTag> builder)
    {
        base.Configure(builder);

        builder.ToTable("FaqTags");

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.Property(p => p.FaqId)
            .IsRequired();

        builder.Property(p => p.TagId)
            .IsRequired();

        builder.HasIndex(p => new { p.FaqId, p.TagId })
            .HasDatabaseName("IX_FaqTag_FaqId_TagId")
            .IsUnique();
    }
}