using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Common.Enums;
using Microsoft.Extensions.Caching.Distributed;

namespace BaseFaq.Common.Infrastructure.Core.Services;

public sealed class RedisTenantSessionStore(IDistributedCache cache) : ITenantSessionStore
{
    private const string KeyPrefix = "tenant-session:";
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(30);

    public Guid? GetTenantId(string externalUserId, AppEnum app)
    {
        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            return null;
        }

        var value = cache.GetString(BuildKey(externalUserId, app));
        return Guid.TryParse(value, out var tenantId)
            ? tenantId
            : null;
    }

    public void SetTenantId(string externalUserId, AppEnum app, Guid tenantId)
    {
        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            return;
        }

        cache.SetString(
            BuildKey(externalUserId, app),
            tenantId.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = Ttl
            });
    }

    public void RemoveTenantId(string externalUserId, AppEnum app)
    {
        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            return;
        }

        cache.Remove(BuildKey(externalUserId, app));
    }

    private static string BuildKey(string externalUserId, AppEnum app)
    {
        return $"{KeyPrefix}{externalUserId}:{app}";
    }
}