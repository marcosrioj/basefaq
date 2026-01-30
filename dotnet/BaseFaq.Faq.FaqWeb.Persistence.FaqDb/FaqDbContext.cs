using BaseFaq.Common.EntityFramework.Core;
using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb;

public class FaqDbContext(
    DbContextOptions<FaqDbContext> options,
    ISessionService sessionService,
    IConfiguration configuration,
    IMemoryCache memoryCache,
    ITenantConnectionStringProvider tenantConnectionStringProvider)
    : BaseDbContext<FaqDbContext>(
        options,
        sessionService,
        configuration,
        memoryCache,
        tenantConnectionStringProvider)
{
    public DbSet<FaqWeb.Persistence.FaqDb.Entities.Faq> Faqs { get; set; }

    public DbSet<FaqItem> FaqItems { get; set; }

    protected override string ConfigurationNamespace =>
        "BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Configurations";

    protected override IEnumerable<string> ConfigurationNamespaces =>
    [
        ConfigurationNamespace,
        "BaseFaq.Common.EntityFramework.Tenant.Configurations"
    ];
}