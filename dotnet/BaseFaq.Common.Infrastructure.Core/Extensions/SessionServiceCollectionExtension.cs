using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Options;
using BaseFaq.Common.Infrastructure.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Common.Infrastructure.Core.Extensions;

public static class SessionServiceCollectionExtension
{
    public static IServiceCollection AddSessionService(
        this IServiceCollection services,
        Action<SessionOptions>? configure = null)
    {
        services.AddHttpContextAccessor();
        services.AddOptions<SessionOptions>();

        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.AddScoped<ISessionService, SessionService>();

        return services;
    }
}