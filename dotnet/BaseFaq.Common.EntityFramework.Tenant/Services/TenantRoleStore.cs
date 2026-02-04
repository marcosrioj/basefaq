using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Tenant.Enums;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Common.EntityFramework.Tenant.Services;

public sealed class TenantRoleStore(TenantDbContext dbContext) : ITenantRoleStore
{
    public async Task<UserRoleType?> GetRoleAsync(Guid tenantId, string userId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        return await dbContext.Users
            .AsNoTracking()
            .Where(user => user.TenantId == tenantId && user.ExternalId == userId)
            .Select(user => (UserRoleType?)user.Role)
            .FirstOrDefaultAsync(cancellationToken);
    }
}