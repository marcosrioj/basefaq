using BaseFaq.Common.EntityFramework.Core.Configurations;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Configurations;

public class FaqContentRefConfiguration : BaseConfiguration<FaqContentRef>
{
    public override void Configure(EntityTypeBuilder<FaqContentRef> builder)
    {
        base.Configure(builder);

        builder.ToTable("FaqContentRefs");

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.Property(p => p.FaqId)
            .IsRequired();

        builder.Property(p => p.ContentRefId)
            .IsRequired();

        builder.HasIndex(p => new { p.FaqId, p.ContentRefId })
            .HasDatabaseName("IX_FaqContentRef_FaqId_ContentRefId")
            .IsUnique();
    }
}