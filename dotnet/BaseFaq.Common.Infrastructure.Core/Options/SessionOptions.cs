using System.Security.Claims;

namespace BaseFaq.Common.Infrastructure.Core.Options;

public class SessionOptions
{
    public const string Name = "Session";
    public string TenantIdClaimType { get; set; } = "tenant_id";
    public string UserIdClaimType { get; set; } = ClaimTypes.NameIdentifier;
    public string TenantIdHeaderName { get; set; } = "X-Tenant-Id";
}