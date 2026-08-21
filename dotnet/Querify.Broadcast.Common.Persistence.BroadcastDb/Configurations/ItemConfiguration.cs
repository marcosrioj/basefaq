using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Querify.Broadcast.Common.Domain.Entities;
using Querify.Common.EntityFramework.Core.Configurations;

namespace Querify.Broadcast.Common.Persistence.BroadcastDb.Configurations;

public sealed class ItemConfiguration : BaseConfiguration<Item>
{
    public override void Configure(EntityTypeBuilder<Item> builder)
    {
        base.Configure(builder);

        builder.ToTable("Items");
        builder.Property(item => item.Kind).HasConversion<int>().IsRequired();
        builder.Property(item => item.ActorKind).HasConversion<int>().IsRequired();
        builder.Property(item => item.Body).HasMaxLength(Item.MaxBodyLength).IsRequired();
        builder.Property(item => item.CapturedAtUtc).IsRequired();
        builder.Property(item => item.ThreadId).IsRequired();
        builder.Property(item => item.TenantId).IsRequired();

        builder.HasIndex(item => new { item.TenantId, item.ThreadId, item.CapturedAtUtc })
            .HasDatabaseName("IX_Item_TenantId_ThreadId_CapturedAtUtc");
    }
}
