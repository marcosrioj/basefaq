using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Common.Infrastructure.Core.Extensions;

public static class SessionServiceCollectionExtension
{
    public static IServiceCollection AddSessionService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        var host = configuration["Redis:Host"];
        var port = configuration["Redis:Port"];
        var password = configuration["Redis:Password"];
        var useSsl = configuration.GetValue("Redis:UseSsl", false);

        if (string.IsNullOrWhiteSpace(host))
        {
            throw new InvalidOperationException("Redis host is missing. Set Redis:Host.");
        }

        if (string.IsNullOrWhiteSpace(port))
        {
            throw new InvalidOperationException("Redis port is missing. Set Redis:Port.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Redis password is missing. Set Redis:Password.");
        }

        var connectionString = $"{host}:{port},password={password},ssl={useSsl},abortConnect=false";

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = "basefaq:";
        });

        services.AddSingleton<ITenantSessionStore, RedisTenantSessionStore>();
        services.AddScoped<ISessionService, SessionService>();

        return services;
    }
}