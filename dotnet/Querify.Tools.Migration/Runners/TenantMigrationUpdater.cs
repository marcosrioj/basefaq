using Querify.Common.EntityFramework.Tenant;
using Querify.Common.EntityFramework.Core;
using Querify.Broadcast.Common.Persistence.BroadcastDb.DbContext;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.QnA.Common.Persistence.QnADb.DbContext;
using Querify.Tools.Migration.Services;
using Querify.Models.Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Querify.Tools.Migration.Runners;

internal static class TenantMigrationUpdater
{
    public static void ApplyAll(IConfiguration configuration, string tenantDbConnectionString, ModuleEnum module)
    {
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
            .Where(item => item.Module == module)
            .Select(item => item.ConnectionString)
            .ToList()
            .Concat(
                tenantDbContext.TenantConnections
                    .AsNoTracking()
                    .Where(item => item.Module == module && item.IsCurrent)
                    .Select(item => item.ConnectionString)
                    .ToList())
            .ToList();

        if (tenantConnectionStrings.Count == 0)
        {
            Console.WriteLine($"No tenant metadata found for {module}.");
            return;
        }

        var uniqueConnections = tenantConnectionStrings
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (uniqueConnections.Count == 0)
        {
            Console.WriteLine($"No valid connection strings found for {module}.");
            return;
        }

        Console.WriteLine($"Applying migrations for {module} ({uniqueConnections.Count} database(s))...");

        var index = 1;
        foreach (var connectionString in uniqueConnections)
        {
            Console.WriteLine($"[{index}/{uniqueConnections.Count}] Updating tenant database...");
            ApplyMigration(module, connectionString, sessionService, configuration, tenantConnectionProvider, httpContextAccessor);
            index++;
        }

        Console.WriteLine("Database update completed.");
    }

    private static void ApplyMigration(
        ModuleEnum module,
        string connectionString,
        MigrationsSessionService sessionService,
        IConfiguration configuration,
        NoopTenantConnectionStringProvider tenantConnectionProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        PostgresDatabaseProvisioner.EnsureDatabaseExists(connectionString);

        switch (module)
        {
            case ModuleEnum.QnA:
            {
                var options = new DbContextOptionsBuilder<QnADbContext>()
                    .UseNpgsql(connectionString)
                    .Options;

                using var dbContext = new QnADbContext(
                    options,
                    sessionService,
                    configuration,
                    tenantConnectionProvider,
                    httpContextAccessor);

                dbContext.Database.Migrate();
                break;
            }
            case ModuleEnum.Direct:
            {
                var options = new DbContextOptionsBuilder<DirectDbContext>()
                    .UseNpgsql(connectionString)
                    .Options;

                using var dbContext = new DirectDbContext(
                    options,
                    sessionService,
                    configuration,
                    tenantConnectionProvider,
                    httpContextAccessor);

                dbContext.Database.Migrate();
                break;
            }
            case ModuleEnum.Broadcast:
            {
                var options = new DbContextOptionsBuilder<BroadcastDbContext>()
                    .UseNpgsql(connectionString)
                    .Options;

                using var dbContext = new BroadcastDbContext(
                    options,
                    sessionService,
                    configuration,
                    tenantConnectionProvider,
                    httpContextAccessor);

                dbContext.Database.Migrate();
                break;
            }
            default:
                throw new InvalidOperationException($"Database update is not supported for {module}.");
        }
    }
}
