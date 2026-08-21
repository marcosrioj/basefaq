using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Models.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace Querify.Common.EntityFramework.Tenant.Providers;

public sealed class AllowedTenantProvider(TenantDbContext tenantDbContext) : IAllowedTenantProvider
{
    public async Task<IReadOnlyDictionary<string, IReadOnlyCollection<Guid>>> GetAllowedTenantIds(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var tenantIds = await tenantDbContext.TenantUsers
            .AsNoTracking()
            .Where(entity => entity.UserId == userId && entity.Tenant.IsActive)
            .Select(entity => entity.TenantId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return Enum.GetValues<ModuleEnum>()
            .ToDictionary(
                module => module.ToString(),
                _ => (IReadOnlyCollection<Guid>)tenantIds);
    }
}
