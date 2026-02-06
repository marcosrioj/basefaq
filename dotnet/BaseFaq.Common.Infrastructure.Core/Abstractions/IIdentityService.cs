using System.Security.Claims;

namespace BaseFaq.Common.Infrastructure.Core.Abstractions;

public interface IIdentityService
{
    string? GetUserIdentity();

    ClaimsPrincipal? GetUser();

    string? GetClientId();

    string? GetName();

    string? GetEmail();
}