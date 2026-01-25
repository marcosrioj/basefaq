using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Identity.Persistence.IdentityDb.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddIdentityDb(this IServiceCollection services, string connectionString)
    {
        Console.WriteLine($"AddIdentityPersistedGrantDb Connection String: {connectionString}");

        var migrationsAssembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name;

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,
                b => b.EnableRetryOnFailure().MigrationsAssembly(migrationsAssembly)));
    }
}