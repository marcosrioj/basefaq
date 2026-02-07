using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Tenant.Providers;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFaqDb(this IServiceCollection services)
    {
        services.AddDbContext<FaqDbContext>();

        services.AddMemoryCache();

        services.AddScoped<ITenantConnectionStringProvider, TenantConnectionStringProvider>();
        services.AddScoped<IUserIdProvider, UserIdProvider>();

        return services;
    }
}