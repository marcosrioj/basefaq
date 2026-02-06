using BaseFaq.Common.EntityFramework.Core;
using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Tenant.Security;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace BaseFaq.Common.EntityFramework.Tenant;

public class TenantDbContext(
    DbContextOptions<TenantDbContext> options,
    ISessionService sessionService,
    IConfiguration configuration,
    IMemoryCache memoryCache,
    ITenantConnectionStringProvider tenantConnectionStringProvider)
    : BaseDbContext<TenantDbContext>(
        options,
        sessionService,
        configuration,
        memoryCache,
        tenantConnectionStringProvider)
{
    public DbSet<Entities.Tenant> Tenants { get; set; } = null!;
    public DbSet<Entities.TenantConnection> TenantConnections { get; set; } = null!;
    public DbSet<Entities.User> Users { get; set; } = null!;

    protected override string ConfigurationNamespace =>
        "BaseFaq.Common.EntityFramework.Tenant.Configurations";

    protected override bool UseTenantConnectionString => false;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var converter = new ValueConverter<string, string>(
            value => EncryptConnectionString(value),
            value => DecryptConnectionString(value));

        modelBuilder.Entity<Entities.Tenant>()
            .Property(tenant => tenant.ConnectionString)
            .HasConversion(converter);

        modelBuilder.Entity<Entities.TenantConnection>()
            .Property(connection => connection.ConnectionString)
            .HasConversion(converter);
    }

    private static string EncryptConnectionString(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || IsEncrypted(value))
        {
            return value;
        }

        return StringCipher.Instance.Encrypt(value) ?? string.Empty;
    }

    private static string DecryptConnectionString(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !IsEncrypted(value))
        {
            return value;
        }

        return StringCipher.Instance.Decrypt(value) ?? string.Empty;
    }

    private static bool IsEncrypted(string? value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               value.StartsWith("v1:", StringComparison.Ordinal);
    }
}