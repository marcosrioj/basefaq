using BaseFaq.Common.Infrastructure.Core.Abstractions;

namespace BaseFaq.Tools.Seed.Infrastructure;

public sealed class StaticTenantConnectionStringProvider(string connectionString) : ITenantConnectionStringProvider
{
    public string GetConnectionString(Guid tenantId)
    {
        return connectionString;
    }
}