using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Querify.Common.EntityFramework.Core.Configurations;
using Querify.Common.EntityFramework.Tenant.Entities;

namespace Querify.Common.EntityFramework.Tenant.Configurations;

public sealed class ChannelConnectionConfiguration : BaseConfiguration<ChannelConnection>
{
    public override void Configure(EntityTypeBuilder<ChannelConnection> builder)
    {
        base.Configure(builder);

        builder.ToTable("ChannelConnections");

        builder.Property(connection => connection.Name)
            .HasMaxLength(ChannelConnection.MaxNameLength)
            .IsRequired();

        builder.Property(connection => connection.ProviderKey)
            .HasMaxLength(ChannelConnection.MaxProviderKeyLength)
            .IsRequired();

        builder.Property(connection => connection.Kind)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(connection => connection.ConnectionData)
            .HasMaxLength(ChannelConnection.MaxConnectionDataLength)
            .IsRequired();

        builder.Property(connection => connection.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(connection => connection.IsEnabled)
            .IsRequired();

        builder.Property(connection => connection.LastErrorMessage)
            .HasMaxLength(ChannelConnection.MaxLastErrorMessageLength);

        builder.Property(connection => connection.TenantId)
            .IsRequired();

        builder.HasOne<Entities.Tenant>()
            .WithMany()
            .HasForeignKey(connection => connection.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(connection => new { connection.TenantId, connection.ProviderKey })
            .IsUnique()
            .HasDatabaseName("IX_ChannelConnection_TenantId_ProviderKey");

        builder.HasIndex(connection => new
            {
                connection.TenantId,
                connection.IsEnabled,
                connection.Status,
                connection.Kind
            })
            .HasDatabaseName("IX_ChannelConnection_TenantId_IsEnabled_Status_Kind");
    }
}
