using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.AI.Common.Persistence.AiDb.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAiDb(this IServiceCollection services, string? connectionString)
    {
        var migrationsAssembly = typeof(AiDbContext).Assembly.GetName().Name;

        services.AddDbContext<AiDbContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.EnableRetryOnFailure().MigrationsAssembly(migrationsAssembly)));

        return services;
    }
}