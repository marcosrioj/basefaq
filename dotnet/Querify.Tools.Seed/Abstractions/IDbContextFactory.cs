using Querify.Common.EntityFramework.Tenant;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Broadcast.Common.Persistence.BroadcastDb.DbContext;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.QnA.Common.Persistence.QnADb.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Querify.Tools.Seed.Abstractions;

public interface IDbContextFactory
{
    TenantDbContext CreateTenantDbContext(
        string connectionString,
        IConfiguration configuration,
        ISessionService sessionService,
        ITenantConnectionStringProvider tenantConnectionStringProvider,
        IHttpContextAccessor httpContextAccessor);

    QnADbContext CreateQnADbContext(
        string connectionString,
        IConfiguration configuration,
        ISessionService sessionService,
        ITenantConnectionStringProvider tenantConnectionStringProvider,
        IHttpContextAccessor httpContextAccessor);

    DirectDbContext CreateDirectDbContext(
        string connectionString,
        IConfiguration configuration,
        ISessionService sessionService,
        ITenantConnectionStringProvider tenantConnectionStringProvider,
        IHttpContextAccessor httpContextAccessor);

    BroadcastDbContext CreateBroadcastDbContext(
        string connectionString,
        IConfiguration configuration,
        ISessionService sessionService,
        ITenantConnectionStringProvider tenantConnectionStringProvider,
        IHttpContextAccessor httpContextAccessor);
}
