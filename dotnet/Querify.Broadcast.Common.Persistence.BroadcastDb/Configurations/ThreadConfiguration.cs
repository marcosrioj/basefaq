using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Querify.Broadcast.Common.Domain.Entities;
using Querify.Common.EntityFramework.Core.Configurations;
using BroadcastThread = Querify.Broadcast.Common.Domain.Entities.Thread;

namespace Querify.Broadcast.Common.Persistence.BroadcastDb.Configurations;

public sealed class ThreadConfiguration : BaseConfiguration<BroadcastThread>
{
    public override void Configure(EntityTypeBuilder<BroadcastThread> builder)
    {
        base.Configure(builder);

        builder.ToTable("Threads");
        builder.Property(thread => thread.Title).HasMaxLength(BroadcastThread.MaxTitleLength);
        builder.Property(thread => thread.Status).HasConversion<int>().IsRequired();
        builder.Property(thread => thread.ChannelConnectionId).IsRequired();
        builder.Property(thread => thread.TenantId).IsRequired();

        builder.HasMany(thread => thread.Items)
            .WithOne(item => item.Thread)
            .HasForeignKey(item => item.ThreadId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(thread => new { thread.TenantId, thread.Status })
            .HasDatabaseName("IX_Thread_TenantId_Status");
        builder.HasIndex(thread => new { thread.TenantId, thread.ChannelConnectionId })
            .HasDatabaseName("IX_Thread_TenantId_ChannelConnectionId");
    }
}
