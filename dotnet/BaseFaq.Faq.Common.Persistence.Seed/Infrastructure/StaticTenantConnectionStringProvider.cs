using BaseFaq.Common.Infrastructure.Core.Abstractions;

namespace BaseFaq.Faq.Common.Persistence.Seed.Infrastructure;

public sealed class StaticTenantConnectionStringProvider(string connectionString) : ITenantConnectionStringProvider
{
    public string GetConnectionString(Guid tenantId)
    {
        return connectionString;
    }
}