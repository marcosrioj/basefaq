using System;
using System.Security.Claims;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using Microsoft.AspNetCore.Http;

namespace BaseFaq.Common.Infrastructure.Core.Services;

public class IdentityService(IHttpContextAccessor httpContextAccessor) : IIdentityService
{
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