using BaseFaq.Models.Common.Enums;

namespace BaseFaq.Common.Infrastructure.Core.Abstractions;

public interface ISessionService
{
    Guid GetTenantId(AppEnum app);
    void Set(Guid tenantId, AppEnum app, string externalUserId);
    void Clear();
}