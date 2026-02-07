using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Common.Enums;
using Microsoft.Extensions.Caching.Distributed;

namespace BaseFaq.Common.Infrastructure.Core.Services;

public sealed class RedisTenantSessionStore(IDistributedCache cache) : ITenantSessionStore
{
    private const string KeyPrefix = "tenant-session:";
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(30);

    public Guid? GetTenantId(Guid userId, AppEnum app)
    {
        if (userId == Guid.Empty)
        {
            return null;
        }

        var value = cache.GetString(BuildKey(userId, app));
        return Guid.TryParse(value, out var tenantId)
            ? tenantId
            : null;
    }

    public void SetTenantId(Guid userId, AppEnum app, Guid tenantId)
    {
        if (userId == Guid.Empty)
        {
            return;
        }

        cache.SetString(
            BuildKey(userId, app),
            tenantId.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = Ttl
            });
    }

    public void RemoveTenantId(Guid userId, AppEnum app)
    {
        if (userId == Guid.Empty)
        {
            return;
        }

        cache.Remove(BuildKey(userId, app));
    }

    private static string BuildKey(Guid userId, AppEnum app)
    {
        return $"{KeyPrefix}{userId}:{app}";
    }
}