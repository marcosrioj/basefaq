using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Common.Enums;

namespace BaseFaq.Faq.Public.Test.IntegrationTests.Helpers;

public sealed class TestSessionService(Guid tenantId, Guid userId) : ISessionService
{
    public Guid GetTenantId(AppEnum app) => tenantId;

    public Guid GetUserId() => userId;
}