using BaseFaq.Common.Infrastructure.Core.Abstractions;

namespace BaseFaq.Common.Infrastructure.Core.Services;

public sealed class SessionService : ISessionService
{
    private readonly IClaimService _claimService;
    private readonly ITenantSessionStore _tenantSessionStore;

    public SessionService(IClaimService claimService, ITenantSessionStore tenantSessionStore)
    {
        _claimService = claimService;
        _tenantSessionStore = tenantSessionStore;
    }

    public Guid? TenantId
    {
        get
        {
            var externalUserId = _claimService.GetExternalUserId();
            return string.IsNullOrWhiteSpace(externalUserId)
                ? null
                : _tenantSessionStore.GetTenantId(externalUserId);
        }
    }

    public void Set(Guid? tenantId, string? externalUserId)
    {
        if (tenantId.HasValue && !string.IsNullOrWhiteSpace(externalUserId))
        {
            _tenantSessionStore.SetTenantId(externalUserId, tenantId.Value);
        }
    }

    public void Clear()
    {
        var externalUserId = _claimService.GetExternalUserId();

        if (!string.IsNullOrWhiteSpace(externalUserId))
        {
            _tenantSessionStore.RemoveTenantId(externalUserId);
        }
    }
}