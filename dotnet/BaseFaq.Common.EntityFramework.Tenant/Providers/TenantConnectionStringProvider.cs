using BaseFaq.Common.Infrastructure.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Common.EntityFramework.Tenant.Providers;

public sealed class TenantConnectionStringProvider(IServiceProvider serviceProvider)
    : ITenantConnectionStringProvider
{
    public string GetConnectionString(Guid tenantId)
    {
        var tenantDbContext = serviceProvider.GetRequiredService<TenantDbContext>();
        var decryptedConnectionString = tenantDbContext.GetTenantConnectionString(tenantId).GetAwaiter().GetResult();

        return decryptedConnectionString;
    }
}