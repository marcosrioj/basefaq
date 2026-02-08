using BaseFaq.Common.EntityFramework.Migrations.Configuration;
using BaseFaq.Common.EntityFramework.Migrations.Services;
using BaseFaq.Common.EntityFramework.Migrations.Utilities;
using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Models.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BaseFaq.Common.EntityFramework.Migrations.Factories;

public sealed class FaqDbContextFactory : IDesignTimeDbContextFactory<FaqDbContext>
{
    public FaqDbContext CreateDbContext(string[] args)
    {
        var configuration = MigrationsConfiguration.Build(SolutionRootLocator.Find());
        var app = ResolveAppEnum(args);
        if (app != AppEnum.FaqWeb)
        {
            throw new InvalidOperationException(
                $"App '{app}' is not supported by {nameof(FaqDbContextFactory)}.");
        }

        var tenantDbConnectionString = MigrationsConfiguration.GetTenantDbConnectionString(configuration);
        var sessionService = new MigrationsSessionService();
        var tenantConnectionProvider = new NoopTenantConnectionStringProvider();

        using var tenantDbContext = new TenantDbContext(
            new DbContextOptionsBuilder<TenantDbContext>()
                .UseNpgsql(tenantDbConnectionString)
                .Options,
            sessionService,
            configuration,
            tenantConnectionProvider);

        var tenantConnection = tenantDbContext
            .GetCurrentTenantConnection(app)
            .GetAwaiter()
            .GetResult();

        var options = new DbContextOptionsBuilder<FaqDbContext>()
            .UseNpgsql(tenantConnection.ConnectionString)
            .Options;

        return new FaqDbContext(options, sessionService, configuration, tenantConnectionProvider);
    }

    private static AppEnum ResolveAppEnum(string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--app", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                return ParseAppEnum(args[i + 1]);
            }

            if (arg.StartsWith("--app=", StringComparison.OrdinalIgnoreCase))
            {
                return ParseAppEnum(arg["--app=".Length..]);
            }
        }

        return AppEnum.FaqWeb;
    }

    private static AppEnum ParseAppEnum(string value)
    {
        if (!Enum.TryParse<AppEnum>(value, ignoreCase: true, out var app))
        {
            throw new InvalidOperationException($"Unknown app value '{value}'.");
        }

        return app;
    }
}