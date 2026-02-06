using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Tenant;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Common.EntityFramework.Tenant.Providers;

public sealed class TenantConnectionStringProvider(IServiceProvider serviceProvider)
    : ITenantConnectionStringProvider
{
    public string GetConnectionString(Guid tenantId)
    {
        var tenantDbContext = serviceProvider.GetRequiredService<TenantDbContext>();
        var connection = tenantDbContext.GetTenantConnection(tenantId).GetAwaiter().GetResult();
        var decryptedConnectionString = connection.ConnectionString;

        if (string.IsNullOrWhiteSpace(decryptedConnectionString))
        {
            throw new InvalidOperationException(
                $"Tenant '{tenantId}' has an invalid connection string.");
        }

        return decryptedConnectionString;
    }
}