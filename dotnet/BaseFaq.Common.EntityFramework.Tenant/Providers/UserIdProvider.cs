using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace BaseFaq.Common.EntityFramework.Tenant.Providers;

public sealed class UserIdProvider(IServiceProvider serviceProvider) : IUserIdProvider
{
    private static readonly TimeSpan UserIdCacheDuration = TimeSpan.FromMinutes(30);

    public Guid GetUserId()
    {
        var tenantDbContext = serviceProvider.GetRequiredService<TenantDbContext>();
        var claimService = serviceProvider.GetRequiredService<IClaimService>();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var externalUserId = claimService.GetExternalUserId();
        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            throw new ApiErrorException(
                "External user ID is missing from the current session.",
                errorCode: (int)HttpStatusCode.Unauthorized);
        }

        var cacheKey = $"UserId:{externalUserId}";
        var userId = memoryCache.GetOrCreate(
            cacheKey,
            entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = UserIdCacheDuration;
                var userName = claimService.GetName();
                var email = claimService.GetEmail();
                return tenantDbContext.EnsureUser(externalUserId, userName, email).GetAwaiter().GetResult();
            });

        return userId;
    }
}