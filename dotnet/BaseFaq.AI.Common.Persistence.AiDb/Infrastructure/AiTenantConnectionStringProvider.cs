using BaseFaq.Common.Infrastructure.Core.Abstractions;

namespace BaseFaq.AI.Common.Persistence.AiDb.Infrastructure;

internal sealed class AiTenantConnectionStringProvider : ITenantConnectionStringProvider
{
    public string GetConnectionString(Guid tenantId)
    {
        throw new InvalidOperationException(
            "Tenant connection strings are not used by AiDbContext.");
    }
}