using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Common.Infrastructure.Core.Extensions;

public static class SessionServiceCollectionExtension
{
    public static IServiceCollection AddSessionService(
        this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ISessionService, SessionService>();

        return services;
    }
}