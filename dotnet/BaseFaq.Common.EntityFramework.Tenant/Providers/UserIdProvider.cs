using System.Net;
using BaseFaq.Common.EntityFramework.Tenant.Entities;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Common.EntityFramework.Tenant.Providers;

public sealed class UserIdProvider(IServiceProvider serviceProvider) : IUserIdProvider
{
    private static readonly TimeSpan UserIdCacheDuration = TimeSpan.FromMinutes(30);

    public Guid GetUserId()
    {
        var claimService = serviceProvider.GetRequiredService<IClaimService>();
        var externalUserId = claimService.GetExternalUserId();
        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            throw new ApiErrorException(
                "External user ID is missing from the current session.",
                errorCode: (int)HttpStatusCode.Unauthorized);
        }

        var cacheKey = $"UserId:{externalUserId}";
        var tenantDbContext = serviceProvider.GetRequiredService<TenantDbContext>();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

        //short‑circuit if it is already being created
        if (TryResolvePendingUserId(tenantDbContext, externalUserId, out var pendingUserId))
        {
            memoryCache.Set(cacheKey, pendingUserId, UserIdCacheDuration);
            return pendingUserId;
        }

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


    private static bool TryResolvePendingUserId(
        TenantDbContext tenantDbContext,
        string externalUserId,
        out Guid userId)
    {
        userId = tenantDbContext.ChangeTracker
            .Entries<User>()
            .FirstOrDefault(entry =>
                entry.State == EntityState.Added &&
                entry.Entity.ExternalId == externalUserId)
            ?.Entity.Id ?? Guid.Empty;

        return userId != Guid.Empty;
    }
}