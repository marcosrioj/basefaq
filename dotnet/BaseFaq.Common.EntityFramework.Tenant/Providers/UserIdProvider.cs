using System.Net;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Common.EntityFramework.Tenant.Providers;

public sealed class UserIdProvider(IServiceProvider serviceProvider) : IUserIdProvider
{
    private static readonly TimeSpan UserIdCacheDuration = TimeSpan.FromMinutes(30);

    public Guid GetUserId(string externalUserId)
    {
        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            throw new ApiErrorException($"User with external id '{externalUserId}' required.",
                (int)HttpStatusCode.BadRequest);
        }

        var tenantDbContext = serviceProvider.GetRequiredService<TenantDbContext>();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var cacheKey = $"UserId:{externalUserId}";
        var userId = memoryCache.GetOrCreate(
            cacheKey,
            entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = UserIdCacheDuration;
                return tenantDbContext.GetUserId(externalUserId).GetAwaiter().GetResult();
            });

        return userId;
    }
}