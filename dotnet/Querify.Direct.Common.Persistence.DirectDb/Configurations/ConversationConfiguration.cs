using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Querify.Common.EntityFramework.Core.Configurations;
using Querify.Direct.Common.Domain.Entities;

namespace Querify.Direct.Common.Persistence.DirectDb.Configurations;

public sealed class ConversationConfiguration : BaseConfiguration<Conversation>
{
    public override void Configure(EntityTypeBuilder<Conversation> builder)
    {
        base.Configure(builder);

        builder.ToTable("Conversations");
        builder.Property(conversation => conversation.Subject).HasMaxLength(Conversation.MaxSubjectLength);
        builder.Property(conversation => conversation.Status).HasConversion<int>().IsRequired();
        builder.Property(conversation => conversation.ContactId).IsRequired();
        builder.Property(conversation => conversation.ChannelConnectionId).IsRequired();
        builder.Property(conversation => conversation.TenantId).IsRequired();

        builder.HasOne(conversation => conversation.Contact)
            .WithMany(contact => contact.Conversations)
            .HasForeignKey(conversation => conversation.ContactId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(conversation => conversation.Messages)
            .WithOne(message => message.Conversation)
            .HasForeignKey(message => message.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(conversation => new { conversation.TenantId, conversation.Status })
            .HasDatabaseName("IX_Conversation_TenantId_Status");
        builder.HasIndex(conversation => new { conversation.TenantId, conversation.ContactId })
            .HasDatabaseName("IX_Conversation_TenantId_ContactId");
        builder.HasIndex(conversation => new { conversation.TenantId, conversation.ChannelConnectionId })
            .HasDatabaseName("IX_Conversation_TenantId_ChannelConnectionId");
    }
}
