using BaseFaq.Common.EntityFramework.Core.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseFaq.Faq.Persistence.FaqDb.Configurations;

public class FaqConfiguration : BaseConfiguration<Entities.Faq>
{
    public override void Configure(EntityTypeBuilder<Entities.Faq> builder)
    {
        base.Configure(builder);

        builder.ToTable("Faqs");

        builder.Property(p => p.Name)
            .IsRequired();

        builder.Property(p => p.Language)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.SortType)
            .IsRequired();

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.HasMany(p => p.Items)
            .WithOne()
            .HasForeignKey(p => p.FaqId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}