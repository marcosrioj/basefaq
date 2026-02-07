using System.Net;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Common.EntityFramework.Tenant.Providers;

public sealed class TenantConnectionStringProvider(IServiceProvider serviceProvider)
    : ITenantConnectionStringProvider
{
    private static readonly TimeSpan ConnectionStringCacheDuration = TimeSpan.FromMinutes(30);

    public string GetConnectionString(Guid tenantId)
    {
        if (Guid.Empty == tenantId)
        {
            throw new ApiErrorException($"Tenant ID '{tenantId}' required.",
                (int)HttpStatusCode.BadRequest);
        }

        var tenantDbContext = serviceProvider.GetRequiredService<TenantDbContext>();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var cacheKey = $"TenantConnectionString:{tenantId}";
        var decryptedConnectionString = memoryCache.GetOrCreate(
            cacheKey,
            entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = ConnectionStringCacheDuration;
                return tenantDbContext.GetTenantConnectionString(tenantId).GetAwaiter().GetResult();
            });

        return decryptedConnectionString!;
    }
}