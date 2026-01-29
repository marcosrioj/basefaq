using System.Reflection;
using BaseFaq.Identity.Persistence.IdentityDb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Identity.Persistence.IdentityDb.Extensions;

public static class ServiceCollectionExtensions
{
    private const string _5DaysTokenProviderName = "5DaysProvider";

    public static void AddIdentityDb(this IServiceCollection services, string? connectionString)
    {
        var migrationsAssembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name;

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.EnableRetryOnFailure().MigrationsAssembly(migrationsAssembly)));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Tokens.PasswordResetTokenProvider = _5DaysTokenProviderName;
                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<
                DataProtectorTokenProvider<ApplicationUser>>(_5DaysTokenProviderName); //Added extended token provider
    }
}