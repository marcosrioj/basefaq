using Querify.Broadcast.Common.Persistence.BroadcastDb.DbContext.TenantIntegrity;
using Querify.Broadcast.Common.Domain.Entities;
using Querify.Common.EntityFramework.Core;
using Querify.Common.EntityFramework.Core.Tenant.DbContext.TenantIntegrity;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Models.Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BroadcastThread = Querify.Broadcast.Common.Domain.Entities.Thread;

namespace Querify.Broadcast.Common.Persistence.BroadcastDb.DbContext;

public class BroadcastDbContext : BaseDbContext<BroadcastDbContext>
{
    public BroadcastDbContext(
        DbContextOptions<BroadcastDbContext> options,
        ISessionService sessionService,
        IConfiguration configuration,
        ITenantConnectionStringProvider tenantConnectionStringProvider,
        IHttpContextAccessor httpContextAccessor)
        : base(
            options,
            sessionService,
            configuration,
            tenantConnectionStringProvider,
            httpContextAccessor)
    {
    }

    public DbSet<BroadcastThread> Threads { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;

    protected override IEnumerable<string> ConfigurationNamespaces =>
    [
        "Querify.Broadcast.Common.Persistence.BroadcastDb.Configurations"
    ];

    protected override ModuleEnum SessionModule => ModuleEnum.Broadcast;

    protected override void OnBeforeSaveChangesRules()
    {
        EnsureTenantIntegrity();
    }

    private void EnsureTenantIntegrity()
    {
        var cache = new TenantIntegrityLookupCacheBase(this);
        this.EnsureItemTenantIntegrity(cache);
    }
}
