using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Common.Enums;

namespace BaseFaq.Common.EntityFramework.Migrations.Services;

internal sealed class MigrationsSessionService : ISessionService
{
    private static readonly Guid MigrationTenantId = Guid.Empty;
    private static readonly Guid MigrationUserId = Guid.Empty;

    public Guid GetTenantId(AppEnum app) => MigrationTenantId;

    public Guid GetUserId() => MigrationUserId;

    public void Set(Guid tenantId, AppEnum app, Guid userId)
    {
    }

    public void Clear()
    {
    }
}

internal sealed class NoopTenantConnectionStringProvider : ITenantConnectionStringProvider
{
    public string GetConnectionString(Guid tenantId)
    {
        throw new InvalidOperationException(
            "Tenant connection string provider is not available for design-time migrations.");
    }
}