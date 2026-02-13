using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Tools.Seed.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BaseFaq.Tools.Seed.Infrastructure;

public sealed class DbContextFactory : IDbContextFactory
{
    public TenantDbContext CreateTenantDbContext(
        string connectionString,
        IConfiguration configuration,
        ISessionService sessionService,
        ITenantConnectionStringProvider tenantConnectionStringProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new TenantDbContext(
            options,
            sessionService,
            configuration,
            tenantConnectionStringProvider,
            httpContextAccessor);
    }

    public FaqDbContext CreateFaqDbContext(
        string connectionString,
        IConfiguration configuration,
        ISessionService sessionService,
        ITenantConnectionStringProvider tenantConnectionStringProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        var options = new DbContextOptionsBuilder<FaqDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new FaqDbContext(
            options,
            sessionService,
            configuration,
            tenantConnectionStringProvider,
            httpContextAccessor);
    }
}