using Microsoft.AspNetCore.Http;
using Querify.Common.Architecture.Test.IntegrationTest.Shared.Configuration;
using Querify.Common.Architecture.Test.IntegrationTest.Shared.Database;
using Querify.Common.Architecture.Test.IntegrationTest.Shared.Session;
using Querify.Common.Architecture.Test.IntegrationTest.Shared.Tenancy;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;

namespace Querify.Direct.Portal.Business.Test.IntegrationTests.Helpers;

public sealed class TestContext : IDisposable
{
    private readonly SqliteInMemoryDatabase _database;

    private TestContext(
        DirectDbContext dbContext,
        IntegrationTestSessionService sessionService,
        SqliteInMemoryDatabase database)
    {
        DbContext = dbContext;
        SessionService = sessionService;
        _database = database;
    }

    public DirectDbContext DbContext { get; }
    public IntegrationTestSessionService SessionService { get; }

    public static TestContext Create(Guid? tenantId = null, Guid? userId = null)
    {
        var database = new SqliteInMemoryDatabase();
        var sessionService = new IntegrationTestSessionService(
            tenantId ?? Guid.NewGuid(),
            userId ?? Guid.NewGuid());
        var httpContext = IntegrationTestHttpContextFactory.Create("DirectPortalTest/1.0");
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var configuration = IntegrationTestConfigurationFactory.Create();
        var tenantConnectionStringProvider = new StaticTenantConnectionStringProvider(database.ConnectionString);

        var dbContext = SqliteInMemoryDbContextFactory.Create<DirectDbContext>(
            database,
            options => new DirectDbContext(
                options,
                sessionService,
                configuration,
                tenantConnectionStringProvider,
                httpContextAccessor));

        return new TestContext(dbContext, sessionService, database);
    }

    public void Dispose()
    {
        DbContext.Dispose();
        _database.Dispose();
    }
}
