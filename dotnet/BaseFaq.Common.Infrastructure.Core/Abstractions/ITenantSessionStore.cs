using BaseFaq.Models.Common.Enums;

namespace BaseFaq.Common.Infrastructure.Core.Abstractions;

public interface ITenantSessionStore
{
    Guid? GetTenantId(string externalUserId, AppEnum app);
    void SetTenantId(string externalUserId, AppEnum app, Guid tenantId);
    void RemoveTenantId(string externalUserId, AppEnum app);
}