using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Common.Enums;

namespace BaseFaq.AI.Common.Persistence.AiDb.Infrastructure;

internal sealed class AiDbSessionService : ISessionService
{
    public Guid GetTenantId(AppEnum app) => Guid.Empty;

    public Guid GetUserId() => Guid.Empty;
}