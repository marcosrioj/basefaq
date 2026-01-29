using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Tenant.Providers;
using BaseFaq.Common.Infrastructure.Core.Configuration;
using BaseFaq.Faq.Persistence.FaqDb.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Faq.Persistence.FaqDb.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFaqDb(this IServiceCollection services, string? connectionString)
    {
        var migrationsAssembly = typeof(FaqDbContext).Assembly.GetName().Name;

        services.AddDbContext<FaqDbContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.EnableRetryOnFailure().MigrationsAssembly(migrationsAssembly)));

        services.AddMemoryCache();
        services.AddSessionService();

        services.AddScoped<ITenantConnectionStringProvider, TenantConnectionStringProvider>();
        services.AddScoped<IFaqRepository, FaqRepository>();
        services.AddScoped<IFaqItemRepository, FaqItemRepository>();

        return services;
    }
}