using BaseFaq.Models.Tenant.Enums;
using Microsoft.AspNetCore.Authorization;

namespace BaseFaq.Common.Infrastructure.Core.Authorization;

public sealed class TenantRoleRequirement : IAuthorizationRequirement
{
    public TenantRoleRequirement(IReadOnlyCollection<UserRoleType> allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }

    public IReadOnlyCollection<UserRoleType> AllowedRoles { get; }
}