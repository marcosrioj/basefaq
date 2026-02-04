using BaseFaq.Models.Tenant.Enums;

namespace BaseFaq.Common.Infrastructure.Core.Abstractions;

public interface ITenantRoleStore
{
    Task<UserRoleType?> GetRoleAsync(Guid tenantId, string userId, CancellationToken cancellationToken);
}