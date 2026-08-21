using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Querify.Common.EntityFramework.Core.Configurations;
using Querify.Direct.Common.Domain.Entities;

namespace Querify.Direct.Common.Persistence.DirectDb.Configurations;

public sealed class ConversationMessageConfiguration : BaseConfiguration<ConversationMessage>
{
    public override void Configure(EntityTypeBuilder<ConversationMessage> builder)
    {
        base.Configure(builder);

        builder.ToTable("ConversationMessages");
        builder.Property(message => message.ActorKind).HasConversion<int>().IsRequired();
        builder.Property(message => message.Body).HasMaxLength(ConversationMessage.MaxBodyLength).IsRequired();
        builder.Property(message => message.SentAtUtc).IsRequired();
        builder.Property(message => message.ConversationId).IsRequired();
        builder.Property(message => message.TenantId).IsRequired();

        builder.HasIndex(message => new { message.TenantId, message.ConversationId, message.SentAtUtc })
            .HasDatabaseName("IX_ConversationMessage_TenantId_ConversationId_SentAtUtc");
    }
}
