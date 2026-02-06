using System.Security.Claims;

namespace BaseFaq.Common.Infrastructure.Core.Abstractions;

public interface IIdentityService
{
    string? GetName();

    string? GetEmail();
}