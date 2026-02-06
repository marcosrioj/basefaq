using System;
using System.Security.Claims;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using Microsoft.AspNetCore.Http;

namespace BaseFaq.Common.Infrastructure.Core.Services;

public class IdentityService(IHttpContextAccessor httpContextAccessor) : IIdentityService
{
    public string? GetUserIdentity()
    {
        var user = httpContextAccessor.HttpContext?.User;
        return user?.FindFirstValue("oid") ?? user?.FindFirstValue("sub");
    }

    public ClaimsPrincipal? GetUser()
    {
        return httpContextAccessor.HttpContext?.User;
    }

    public string? GetClientId()
    {
        return httpContextAccessor.HttpContext?.User.FindFirst("client_id")?.Value;
    }

    public string? GetName()
    {
        var user = httpContextAccessor.HttpContext?.User;
        return FindClaimBySuffix(user, "/name") ??
               user?.FindFirstValue("name") ??
               user?.FindFirstValue(ClaimTypes.Name);
    }

    public string? GetEmail()
    {
        var user = httpContextAccessor.HttpContext?.User;
        return FindClaimBySuffix(user, "/email") ??
               user?.FindFirstValue("email") ??
               user?.FindFirstValue(ClaimTypes.Email);
    }

    private static string? FindClaimBySuffix(ClaimsPrincipal? user, string suffix)
    {
        return user?.Claims.FirstOrDefault(claim => claim.Type.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }
}