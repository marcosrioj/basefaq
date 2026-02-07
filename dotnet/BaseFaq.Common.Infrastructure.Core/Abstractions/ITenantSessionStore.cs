using BaseFaq.Models.Common.Enums;

namespace BaseFaq.Common.Infrastructure.Core.Abstractions;

public interface ITenantSessionStore
{
    Guid? GetTenantId(Guid userId, AppEnum app);
    void SetTenantId(Guid userId, AppEnum app, Guid tenantId);
    void RemoveTenantId(Guid userId, AppEnum app);
}