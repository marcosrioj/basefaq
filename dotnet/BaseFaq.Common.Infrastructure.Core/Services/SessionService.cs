using BaseFaq.Common.Infrastructure.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BaseFaq.Common.Infrastructure.Core.Services;

public sealed class SessionService : ISessionService
{
    private const string ExternalUserIdClaimType = ClaimTypes.NameIdentifier;

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantSessionStore _tenantSessionStore;

    public SessionService(IHttpContextAccessor httpContextAccessor, ITenantSessionStore tenantSessionStore)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantSessionStore = tenantSessionStore;
    }

    public Guid? TenantId
    {
        get
        {
            var externalUserId = ExternalUserId;
            return string.IsNullOrWhiteSpace(externalUserId)
                ? null
                : _tenantSessionStore.GetTenantId(externalUserId);
        }
    }

    public string? ExternalUserId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.User?.FindFirst(ExternalUserIdClaimType)?.Value;
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
        if (!string.IsNullOrWhiteSpace(ExternalUserId))
        {
            _tenantSessionStore.RemoveTenantId(ExternalUserId);
        }
    }
}