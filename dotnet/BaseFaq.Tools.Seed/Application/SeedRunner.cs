using BaseFaq.Tools.Seed.Abstractions;
using BaseFaq.Tools.Seed.Configuration;
using BaseFaq.Tools.Seed.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BaseFaq.Tools.Seed.Application;

public sealed class SeedRunner(
    IConfiguration configuration,
    IConsoleAdapter console,
    IDbContextFactory dbContextFactory,
    ITenantSeedService tenantSeeder,
    IFaqSeedService faqSeeder,
    ICleanupService cleanupService,
    SeedCounts counts)
    : ISeedRunner
{
    public int Run()
    {
        var settings = SeedSettings.From(configuration);
        var tenantBuilder = new NpgsqlConnectionStringBuilder(settings.TenantConnectionString);
        var faqBuilder = new NpgsqlConnectionStringBuilder(settings.FaqConnectionString);

        console.WriteLine(
            $"Using TenantDb from appsettings.json: {FormatConnectionInfo(tenantBuilder)}");
        console.WriteLine(
            $"Using FaqDb from appsettings.json: {FormatConnectionInfo(faqBuilder)}");

        var action = PromptAction(console);
        if (action == SeedAction.Exit)
        {
            return 0;
        }

        var seedUserId = Guid.NewGuid();
        var httpContextAccessor = new HttpContextAccessor();

        var tenantSessionService = new SeedSessionService(seedUserId, Guid.Empty);
        var dummyTenantProvider = new StaticTenantConnectionStringProvider(faqBuilder.ToString());

        using var tenantDb = dbContextFactory.CreateTenantDbContext(
            tenantBuilder.ToString(),
            configuration,
            tenantSessionService,
            dummyTenantProvider,
            httpContextAccessor);

        tenantDb.Database.Migrate();

        if (action is SeedAction.CleanOnly or SeedAction.CleanAndSeed)
        {
            cleanupService.CleanTenantDb(tenantDb);
        }

        if (action is SeedAction.SeedOnly or SeedAction.CleanAndSeed)
        {
            if (tenantSeeder.HasData(tenantDb) &&
                !Confirm(console, "Tenant database already has data. Append seed data?"))
            {
                return 0;
            }

            var seedTenantId = tenantSeeder.Seed(
                tenantDb,
                new TenantSeedRequest(tenantBuilder.ToString(), faqBuilder.ToString()),
                counts);

            var faqSessionService = new SeedSessionService(seedUserId, seedTenantId);
            var faqTenantProvider = new StaticTenantConnectionStringProvider(faqBuilder.ToString());

            using var faqDb = dbContextFactory.CreateFaqDbContext(
                faqBuilder.ToString(),
                configuration,
                faqSessionService,
                faqTenantProvider,
                httpContextAccessor);

            faqDb.Database.Migrate();
            faqDb.TenantFiltersEnabled = false;
            faqDb.SoftDeleteFiltersEnabled = false;

            if (action is SeedAction.CleanAndSeed)
            {
                cleanupService.CleanFaqDb(faqDb);
            }

            if (faqSeeder.HasData(faqDb) &&
                !Confirm(console, "FAQ database already has data. Append seed data?"))
            {
                return 0;
            }

            faqSeeder.Seed(faqDb, seedTenantId, counts);
            return 0;
        }

        var cleanFaqSessionService = new SeedSessionService(seedUserId, Guid.Empty);
        var cleanFaqTenantProvider = new StaticTenantConnectionStringProvider(faqBuilder.ToString());

        using var cleanFaqDb = dbContextFactory.CreateFaqDbContext(
            faqBuilder.ToString(),
            configuration,
            cleanFaqSessionService,
            cleanFaqTenantProvider,
            httpContextAccessor);

        cleanFaqDb.Database.Migrate();
        cleanFaqDb.TenantFiltersEnabled = false;
        cleanFaqDb.SoftDeleteFiltersEnabled = false;

        cleanupService.CleanFaqDb(cleanFaqDb);
        return 0;
    }

    private static string FormatConnectionInfo(NpgsqlConnectionStringBuilder builder)
    {
        return $"Host={builder.Host};Port={builder.Port};Database={builder.Database};Username={builder.Username}";
    }

    private static bool Confirm(IConsoleAdapter console, string message)
    {
        console.Write($"{message} (y/N): ");
        var input = console.ReadLine();
        return string.Equals(input, "y", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(input, "yes", StringComparison.OrdinalIgnoreCase);
    }

    private static SeedAction PromptAction(IConsoleAdapter console)
    {
        console.WriteLine("Select action:");
        console.WriteLine("1) Seed data (default)");
        console.WriteLine("2) Clean databases and seed data");
        console.WriteLine("3) Clean databases only");
        console.WriteLine("0) Exit");
        console.Write("Choice: ");
        var input = console.ReadLine();
        return input switch
        {
            "2" => SeedAction.CleanAndSeed,
            "3" => SeedAction.CleanOnly,
            "0" => SeedAction.Exit,
            _ => SeedAction.SeedOnly
        };
    }

    private enum SeedAction
    {
        SeedOnly,
        CleanAndSeed,
        CleanOnly,
        Exit
    }
}