using BaseFaq.Common.EntityFramework.Core.Configurations;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Configurations;

public class VoteConfiguration : BaseConfiguration<Vote>
{
    public override void Configure(EntityTypeBuilder<Vote> builder)
    {
        base.Configure(builder);

        builder.ToTable("Votes");

        builder.Property(p => p.Like)
            .IsRequired();

        builder.Property(p => p.UserPrint)
            .HasMaxLength(Vote.MaxUserPrintLength)
            .IsRequired();

        builder.Property(p => p.Ip)
            .HasMaxLength(Vote.MaxIpLength)
            .IsRequired();

        builder.Property(p => p.UserAgent)
            .HasMaxLength(Vote.MaxUserAgentLength)
            .IsRequired();

        builder.Property(p => p.UnLikeReason);

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.Property(p => p.FaqItemId)
            .IsRequired();

        builder.HasIndex(p => p.FaqItemId)
            .HasDatabaseName("IX_Vote_FaqItemId");
    }
}