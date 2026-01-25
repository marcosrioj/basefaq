using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Common.Infrastructure.Core.Configuration;

public static class ForwardedHeadersConfiguration
{
    public static void AddCustomForwardedHeaders(this IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        });
    }

    public static void UseCustomForwardedHeaders(this IApplicationBuilder app)
    {
        app.UseForwardedHeaders();
    }
}