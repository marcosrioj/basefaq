using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Common.Enums;

namespace BaseFaq.Faq.Common.Persistence.Seed.Infrastructure;

public sealed class SeedSessionService(Guid userId, Guid tenantId) : ISessionService
{
    public Guid GetTenantId(AppEnum app)
    {
        return tenantId;
    }

    public Guid GetUserId()
    {
        return userId;
    }
}