using BaseFaq.Common.EntityFramework.Core.Configurations.Base;
using BaseFaq.Common.EntityFramework.Tenant.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseFaq.Common.EntityFramework.Tenant.Configurations;

public class UserConfiguration : BaseConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users");

        builder.Property(p => p.GivenName)
            .IsRequired();

        builder.Property(p => p.SurName)
            .IsRequired();

        builder.Property(p => p.Role)
            .IsRequired();

        builder.HasIndex(p => p.Email)
            .HasDatabaseName("IX_User_Email");
    }
}