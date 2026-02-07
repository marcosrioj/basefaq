using BaseFaq.Models.Common.Enums;

namespace BaseFaq.Common.Infrastructure.Core.Abstractions;

public interface ISessionService
{
    Guid GetTenantId(AppEnum app);
    Guid GetUserId();
    void Set(Guid tenantId, AppEnum app, Guid userId);
    void Clear();
}