using BaseFaq.AI.Generation.Business.Worker.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BaseFaq.AI.Generation.Business.Worker.Service;

public sealed class FaqIntegrationDbContextFactory(
    ITenantConnectionStringProvider tenantConnectionStringProvider,
    IConfiguration configuration) : IFaqIntegrationDbContextFactory
{
    public FaqDbContext Create(Guid tenantId)
    {
        var connectionString = tenantConnectionStringProvider.GetConnectionString(tenantId);

        var options = new DbContextOptionsBuilder<FaqDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new FaqDbContext(
            options,
            new IntegrationSessionService(tenantId),
            configuration,
            new StaticTenantConnectionStringProvider(connectionString),
            new HttpContextAccessor());
    }

    private sealed class IntegrationSessionService(Guid tenantId) : ISessionService
    {
        public Guid GetTenantId(BaseFaq.Models.Common.Enums.AppEnum app) => tenantId;
        public Guid GetUserId() => Guid.Empty; //TODO: Create a AI User for that
    }

    private sealed class StaticTenantConnectionStringProvider(string connectionString) : ITenantConnectionStringProvider
    {
        public string GetConnectionString(Guid tenantId) => connectionString;
    }
}