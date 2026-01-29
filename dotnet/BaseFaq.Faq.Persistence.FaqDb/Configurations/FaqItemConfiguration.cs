using BaseFaq.Common.EntityFramework.Core.Configurations;
using BaseFaq.Faq.Persistence.FaqDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseFaq.Faq.Persistence.FaqDb.Configurations;

public class FaqItemConfiguration : BaseConfiguration<FaqItem>
{
    public override void Configure(EntityTypeBuilder<FaqItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("FaqItems");

        builder.Property(p => p.Question)
            .IsRequired();

        builder.Property(p => p.Answer)
            .IsRequired();

        builder.Property(p => p.Origin)
            .IsRequired();

        builder.Property(p => p.FaqId)
            .IsRequired();

        builder.HasIndex(p => p.FaqId)
            .HasDatabaseName("IX_FaqItem_FaqId");
    }
}