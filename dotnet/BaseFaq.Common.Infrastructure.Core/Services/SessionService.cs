using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Models.Common.Enums;
using System.Net;

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

    public Guid GetTenantId(AppEnum app)
    {
        var externalUserId = _claimService.GetExternalUserId();
        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            throw new ApiErrorException(
                "External user ID is missing from the current session.",
                errorCode: (int)HttpStatusCode.Unauthorized);
        }

        var tenantId = _tenantSessionStore.GetTenantId(externalUserId, app);
        if (!tenantId.HasValue)
        {
            throw new ApiErrorException(
                "TenantId is missing from the current session.",
                errorCode: (int)HttpStatusCode.UnprocessableEntity);
        }

        return tenantId.Value;
    }

    public void Set(Guid tenantId, AppEnum app, string externalUserId)
    {
        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            return;
        }

        _tenantSessionStore.SetTenantId(externalUserId, app, tenantId);
    }

    public void Clear()
    {
        var externalUserId = _claimService.GetExternalUserId();

        if (!string.IsNullOrWhiteSpace(externalUserId))
        {
            foreach (AppEnum app in Enum.GetValues<AppEnum>())
            {
                _tenantSessionStore.RemoveTenantId(externalUserId, app);
            }
        }
    }
}