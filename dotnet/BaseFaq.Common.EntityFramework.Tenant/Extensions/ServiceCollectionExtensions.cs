using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Tenant.Providers;
using BaseFaq.Common.Infrastructure.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Common.EntityFramework.Tenant.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTenantDb(this IServiceCollection services, string? connectionString)
    {
        var migrationsAssembly = typeof(TenantDbContext).Assembly.GetName().Name;

        services.AddDbContext<TenantDbContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.EnableRetryOnFailure().MigrationsAssembly(migrationsAssembly)));

        services.AddMemoryCache();
        services.AddSessionService();

        services.AddScoped<ITenantConnectionStringProvider, TenantConnectionStringProvider>();

        return services;
    }
}