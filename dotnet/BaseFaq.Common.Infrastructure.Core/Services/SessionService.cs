using System.Net;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Common.Enums;

namespace BaseFaq.Common.Infrastructure.Core.Services;

public sealed class SessionService : ISessionService
{
    private readonly IClaimService _claimService;
    private readonly ITenantSessionStore _tenantSessionStore;
    private readonly IUserIdProvider _userIdProvider;

    public SessionService(
        IClaimService claimService,
        ITenantSessionStore tenantSessionStore,
        IUserIdProvider userIdProvider)
    {
        _claimService = claimService;
        _tenantSessionStore = tenantSessionStore;
        _userIdProvider = userIdProvider;
    }

    public Guid GetTenantId(AppEnum app)
    {
        var userId = GetUserId();
        var tenantId = _tenantSessionStore.GetTenantId(userId, app);
        if (!tenantId.HasValue)
        {
            throw new ApiErrorException(
                "TenantId is missing from the current session.",
                errorCode: (int)HttpStatusCode.UnprocessableEntity);
        }

        return tenantId.Value;
    }

    public Guid GetUserId()
    {
        var externalUserId = _claimService.GetExternalUserId();
        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            throw new ApiErrorException(
                "External user ID is missing from the current session.",
                errorCode: (int)HttpStatusCode.Unauthorized);
        }

        var userId = _userIdProvider.GetUserId(externalUserId);

        return userId;
    }

    public void Set(Guid tenantId, AppEnum app, Guid userId)
    {
        _tenantSessionStore.SetTenantId(userId, app, tenantId);
    }

    public void Clear()
    {
        var userId = GetUserId();

        foreach (AppEnum app in Enum.GetValues<AppEnum>())
        {
            _tenantSessionStore.RemoveTenantId(userId, app);
        }
    }
}