using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

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
            throw new ApiErrorException(
                "Redis host is missing. Set Redis:Host.",
                errorCode: (int)HttpStatusCode.InternalServerError);
        }

        if (string.IsNullOrWhiteSpace(port))
        {
            throw new ApiErrorException(
                "Redis port is missing. Set Redis:Port.",
                errorCode: (int)HttpStatusCode.InternalServerError);
        }

        var connectionString = $"{host}:{port},password={password},ssl={useSsl},abortConnect=false";

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = "basefaq:";
        });

        services.AddClaimService(configuration);

        services.AddSingleton<IAllowedTenantStore, RedisAllowedTenantStore>();

        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IClientKeyContextService, ClientKeyContextService>();

        return services;
    }
}