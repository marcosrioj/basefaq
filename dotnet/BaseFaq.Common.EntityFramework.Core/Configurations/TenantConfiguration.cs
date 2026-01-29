using BaseFaq.Common.EntityFramework.Core.Configurations.Base;
using BaseFaq.Common.EntityFramework.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseFaq.Common.EntityFramework.Core.Configurations;

public class TenantConfiguration : BaseConfiguration<Tenant>
{
    public override void Configure(EntityTypeBuilder<Tenant> builder)
    {
        base.Configure(builder);

        builder.ToTable("Tenants");

        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(Tenant.MaxSlugLength);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(Tenant.MaxNameLength);

        builder.Property(p => p.ConnectionString)
            .IsRequired()
            .HasMaxLength(Tenant.MaxConnectionStringLength);

        builder.Property(p => p.Edition)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.HasIndex(p => p.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Tenant_Slug");
    }
}