using Querify.Broadcast.Common.Persistence.BroadcastDb.DbContext;
using Querify.Common.EntityFramework.Tenant;
using Querify.Common.EntityFramework.Tenant.Entities;
using Querify.Models.Common.Enums;
using Querify.Tools.Migration.Configuration;
using Querify.Tools.Migration.Services;
using Querify.Tools.Migration.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Querify.Tools.Migration.Factories;

public sealed class BroadcastDbContextFactory : IDesignTimeDbContextFactory<BroadcastDbContext>
{
    public BroadcastDbContext CreateDbContext(string[] args)
    {
        var configuration = MigrationsConfiguration.Build(SolutionRootLocator.Find());
        var module = ResolveModuleEnum(args);
        if (module != ModuleEnum.Broadcast)
        {
            throw new InvalidOperationException(
                $"Module '{module}' is not supported by {nameof(BroadcastDbContextFactory)}.");
        }

        var tenantDbConnectionString = MigrationsConfiguration.GetTenantDbConnectionString(configuration);
        var designTimeConnectionString = ResolveDesignTimeConnectionString(configuration, tenantDbConnectionString);
        var sessionService = new MigrationsSessionService();
        var tenantConnectionProvider = new NoopTenantConnectionStringProvider();
        var httpContextAccessor = new HttpContextAccessor();

        TenantConnection tenantConnection;
        try
        {
            using var tenantDbContext = new TenantDbContext(
                new DbContextOptionsBuilder<TenantDbContext>()
                    .UseNpgsql(tenantDbConnectionString)
                    .Options,
                sessionService,
                configuration,
                tenantConnectionProvider,
                httpContextAccessor);

            tenantConnection = ResolveCurrentConnection(tenantDbContext);
        }
        catch when (!string.IsNullOrWhiteSpace(designTimeConnectionString))
        {
            tenantConnection = new TenantConnection
            {
                ConnectionString = designTimeConnectionString,
                Module = module,
                IsCurrent = true
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Failed to connect to the tenant database while creating the Broadcast DbContext. " +
                "Make sure the database is running and ConnectionStrings:TenantDb is set correctly, " +
                "or provide ConnectionStrings:BroadcastDb for offline design-time scaffolding.",
                ex);
        }

        var options = new DbContextOptionsBuilder<BroadcastDbContext>()
            .UseNpgsql(tenantConnection.ConnectionString)
            .Options;

        return new BroadcastDbContext(
            options,
            sessionService,
            configuration,
            tenantConnectionProvider,
            httpContextAccessor);
    }

    private static string ResolveDesignTimeConnectionString(
        IConfiguration configuration,
        string tenantDbConnectionString)
    {
        var broadcastDbConnectionString = configuration.GetConnectionString("BroadcastDb");
        if (!string.IsNullOrWhiteSpace(broadcastDbConnectionString))
        {
            return broadcastDbConnectionString;
        }

        return tenantDbConnectionString;
    }

    private static TenantConnection ResolveCurrentConnection(TenantDbContext tenantDbContext)
    {
        return tenantDbContext
            .GetCurrentTenantConnection(ModuleEnum.Broadcast)
            .GetAwaiter()
            .GetResult();
    }

    private static ModuleEnum ResolveModuleEnum(string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--module", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                return ParseModuleEnum(args[i + 1]);
            }

            if (arg.StartsWith("--module=", StringComparison.OrdinalIgnoreCase))
            {
                return ParseModuleEnum(arg["--module=".Length..]);
            }
        }

        return ModuleEnum.Broadcast;
    }

    private static ModuleEnum ParseModuleEnum(string value)
    {
        if (!Enum.TryParse<ModuleEnum>(value, ignoreCase: true, out var module))
        {
            throw new InvalidOperationException($"Unknown module value '{value}'.");
        }

        return module;
    }
}
