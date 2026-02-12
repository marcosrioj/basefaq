using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BaseFaq.Faq.Common.Persistence.Seed.Abstractions;

public interface IDbContextFactory
{
    TenantDbContext CreateTenantDbContext(
        string connectionString,
        IConfiguration configuration,
        ISessionService sessionService,
        ITenantConnectionStringProvider tenantConnectionStringProvider,
        IHttpContextAccessor httpContextAccessor);

    FaqDbContext CreateFaqDbContext(
        string connectionString,
        IConfiguration configuration,
        ISessionService sessionService,
        ITenantConnectionStringProvider tenantConnectionStringProvider,
        IHttpContextAccessor httpContextAccessor);
}