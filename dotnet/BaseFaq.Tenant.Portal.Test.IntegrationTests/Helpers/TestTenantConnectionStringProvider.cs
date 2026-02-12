using BaseFaq.Common.Infrastructure.Core.Abstractions;

namespace BaseFaq.Tenant.Portal.Test.IntegrationTests.Helpers;

public sealed class TestTenantConnectionStringProvider(string connectionString) : ITenantConnectionStringProvider
{
    public string GetConnectionString(Guid tenantId)
    {
        return connectionString;
    }
}