using BaseFaq.Common.Infrastructure.Core.Middleware;
using BaseFaq.Models.Common.Enums;
using Microsoft.AspNetCore.Builder;

namespace BaseFaq.Common.Infrastructure.Core.Extensions;

public static class TenantResolutionApplicationBuilderExtensions
{
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app, AppEnum appEnum,
        bool enableTenantAccessValidation = true)
    {
        return app.UseWhen(
            context =>
                !context.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase) &&
                !context.Request.Path.StartsWithSegments("/openapi", StringComparison.OrdinalIgnoreCase),
            branch => branch.UseMiddleware<TenantResolutionMiddleware>(new TenantResolutionOptions
            {
                App = appEnum,
                EnableTenantAccessValidation = enableTenantAccessValidation
            }));
    }
}