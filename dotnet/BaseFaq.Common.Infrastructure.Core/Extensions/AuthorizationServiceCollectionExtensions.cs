using BaseFaq.Common.Infrastructure.Core.Authorization;
using BaseFaq.Models.Tenant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Common.Infrastructure.Core.Extensions;

public static class AuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddTenantRoleAuthorization(this IServiceCollection services,
        Action<AuthorizationOptions> configurePolicies)
    {
        services.AddAuthorization(configurePolicies);
        services.AddSingleton<IAuthorizationHandler, TenantRoleAuthorizationHandler>();

        return services;
    }

    public static void AddTenantRolePolicy(this AuthorizationOptions options, string policyName,
        params UserRoleType[] allowedRoles)
    {
        options.AddPolicy(policyName, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new TenantRoleRequirement(allowedRoles));
        });
    }
}