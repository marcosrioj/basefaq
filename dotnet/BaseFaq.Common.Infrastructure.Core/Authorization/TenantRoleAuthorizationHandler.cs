using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Tenant.Enums;
using Microsoft.AspNetCore.Authorization;

namespace BaseFaq.Common.Infrastructure.Core.Authorization;

public sealed class TenantRoleAuthorizationHandler(
    ITenantRoleStore roleStore,
    ISessionService sessionService)
    : AuthorizationHandler<TenantRoleRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        TenantRoleRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var tenantId = sessionService.TenantId;
        var userId = sessionService.UserId;
        if (tenantId is null || string.IsNullOrWhiteSpace(userId))
        {
            return;
        }

        var role = await roleStore.GetRoleAsync(tenantId.Value, userId, CancellationToken.None);
        if (role is null)
        {
            return;
        }

        if (requirement.AllowedRoles.Contains(role.Value))
        {
            context.Succeed(requirement);
        }
    }
}