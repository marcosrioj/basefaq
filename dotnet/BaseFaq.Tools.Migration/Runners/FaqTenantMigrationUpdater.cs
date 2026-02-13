using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Tools.Migration.Services;
using BaseFaq.Models.Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BaseFaq.Tools.Migration.Runners;

internal static class FaqTenantMigrationUpdater
{
    public static void ApplyAll(IConfiguration configuration, string tenantDbConnectionString, AppEnum app)
    {
        if (app != AppEnum.Faq)
        {
            throw new InvalidOperationException(
                $"Database update is only supported for {AppEnum.Faq} at the moment.");
        }

        var sessionService = new MigrationsSessionService();
        var tenantConnectionProvider = new NoopTenantConnectionStringProvider();
        var httpContextAccessor = new HttpContextAccessor();

        using var tenantDbContext = new TenantDbContext(
            new DbContextOptionsBuilder<TenantDbContext>()
                .UseNpgsql(tenantDbConnectionString)
                .Options,
            sessionService,
            configuration,
            tenantConnectionProvider,
            httpContextAccessor);

        var tenantConnectionStrings = tenantDbContext.Tenants
            .AsNoTracking()
            .Where(item => item.App == app)
            .Select(item => item.ConnectionString)
            .ToList();

        if (tenantConnectionStrings.Count == 0)
        {
            Console.WriteLine($"No tenants found for {app}.");
            return;
        }

        var uniqueConnections = tenantConnectionStrings
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (uniqueConnections.Count == 0)
        {
            Console.WriteLine($"No valid connection strings found for {app}.");
            return;
        }

        Console.WriteLine($"Applying migrations for {app} ({uniqueConnections.Count} tenant(s))...");

        var index = 1;
        foreach (var connectionString in uniqueConnections)
        {
            var options = new DbContextOptionsBuilder<FaqDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            using var faqDbContext = new FaqDbContext(
                options,
                sessionService,
                configuration,
                tenantConnectionProvider,
                httpContextAccessor);

            Console.WriteLine($"[{index}/{uniqueConnections.Count}] Updating tenant database...");
            faqDbContext.Database.Migrate();
            index++;
        }

        Console.WriteLine("Database update completed.");
    }
}