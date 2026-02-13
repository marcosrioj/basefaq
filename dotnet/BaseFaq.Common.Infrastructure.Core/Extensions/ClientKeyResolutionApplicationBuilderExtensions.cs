using BaseFaq.Common.Infrastructure.Core.Middleware;
using Microsoft.AspNetCore.Builder;

namespace BaseFaq.Common.Infrastructure.Core.Extensions;

public static class ClientKeyResolutionApplicationBuilderExtensions
{
    public static IApplicationBuilder UseClientKeyResolution(this IApplicationBuilder app)
    {
        return app.UseWhen(
            context =>
                !context.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase) &&
                !context.Request.Path.StartsWithSegments("/openapi", StringComparison.OrdinalIgnoreCase),
            branch => branch.UseMiddleware<ClientKeyResolutionMiddleware>());
    }
}