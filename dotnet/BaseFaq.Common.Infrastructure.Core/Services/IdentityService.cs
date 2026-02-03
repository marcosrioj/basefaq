using System.Security.Claims;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using Microsoft.AspNetCore.Http;

namespace BaseFaq.Common.Infrastructure.Core.Services;

public class IdentityService(IHttpContextAccessor httpContextAccessor) : IIdentityService
{
    public string? GetUserIdentity()
    {
        return httpContextAccessor.HttpContext?.User.FindFirstValue("sub");
    }

    public ClaimsPrincipal? GetUser()
    {
        return httpContextAccessor.HttpContext?.User;
    }

    public string? GetClientId()
    {
        return httpContextAccessor.HttpContext?.User.FindFirst("client_id")?.Value;
    }

    public IList<string> GetScopes()
    {
        var scopeList = httpContextAccessor.HttpContext?.User?.FindFirst("scope")?.Value;
        if (!string.IsNullOrEmpty(scopeList))
        {
            return scopeList.Split(",").ToList();
        }

        return new List<string>();
    }
}