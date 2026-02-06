using BaseFaq.Common.Infrastructure.Core.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace BaseFaq.Common.Infrastructure.Core.Services;

public sealed class RedisTenantSessionStore(IDistributedCache cache) : ITenantSessionStore
{
    private const string KeyPrefix = "tenant-session:";
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(30);

    public Guid? GetTenantId(string externalUserId)
    {
        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            return null;
        }

        var value = cache.GetString(KeyPrefix + externalUserId);
        return Guid.TryParse(value, out var tenantId)
            ? tenantId
            : null;
    }

    public void SetTenantId(string externalUserId, Guid tenantId)
    {
        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            return;
        }

        cache.SetString(
            KeyPrefix + externalUserId,
            tenantId.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = Ttl
            });
    }

    public void RemoveTenantId(string externalUserId)
    {
        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            return;
        }

        cache.Remove(KeyPrefix + externalUserId);
    }
}