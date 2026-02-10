using BaseFaq.Common.EntityFramework.Core.Configurations;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Configurations;

public class ContentRefConfiguration : BaseConfiguration<ContentRef>
{
    public override void Configure(EntityTypeBuilder<ContentRef> builder)
    {
        base.Configure(builder);

        builder.ToTable("ContentRefs");

        builder.Property(p => p.Kind)
            .IsRequired();

        builder.Property(p => p.Locator)
            .HasMaxLength(ContentRef.MaxLocatorLength)
            .IsRequired();

        builder.Property(p => p.Label)
            .HasMaxLength(ContentRef.MaxLabelLength);

        builder.Property(p => p.Scope)
            .HasMaxLength(ContentRef.MaxScopeLength);

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.HasIndex(p => p.Kind)
            .HasDatabaseName("IX_ContentRef_Kind");
    }
}