using Microsoft.AspNetCore.Http;
using Querify.Common.Architecture.Test.IntegrationTest.Shared.Configuration;
using Querify.Common.Architecture.Test.IntegrationTest.Shared.Database;
using Querify.Common.Architecture.Test.IntegrationTest.Shared.Session;
using Querify.Common.EntityFramework.Tenant;

namespace Querify.Common.Architecture.Test.IntegrationTest.Shared.Tenancy;

public sealed class TenantControlPlaneTestContext : IDisposable
{
    private readonly SqliteInMemoryDatabase _database;

    private TenantControlPlaneTestContext(TenantDbContext dbContext, SqliteInMemoryDatabase database)
    {
        DbContext = dbContext;
        _database = database;
    }

    public TenantDbContext DbContext { get; }

    public static TenantControlPlaneTestContext Create(Guid tenantId)
    {
        var database = new SqliteInMemoryDatabase();
        var sessionService = new IntegrationTestSessionService(tenantId, Guid.NewGuid());
        var httpContext = IntegrationTestHttpContextFactory.Create("TenantControlPlaneTest/1.0");
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var configuration = IntegrationTestConfigurationFactory.Create();
        var tenantConnectionStringProvider = new StaticTenantConnectionStringProvider(database.ConnectionString);

        var dbContext = SqliteInMemoryDbContextFactory.Create<TenantDbContext>(
            database,
            options => new TenantDbContext(
                options,
                sessionService,
                configuration,
                tenantConnectionStringProvider,
                httpContextAccessor));

        return new TenantControlPlaneTestContext(dbContext, database);
    }

    public void Dispose()
    {
        DbContext.Dispose();
        _database.Dispose();
    }
}
