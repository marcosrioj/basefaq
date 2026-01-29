using BaseFaq.Common.EntityFramework.Core;
using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
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

    protected override string ConfigurationNamespace =>
        "BaseFaq.Common.EntityFramework.Tenant.Configurations";
}