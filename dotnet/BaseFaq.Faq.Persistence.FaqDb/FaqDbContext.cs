using BaseFaq.Common.EntityFramework.Core;
using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.Persistence.FaqDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace BaseFaq.Faq.Persistence.FaqDb;

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
    public DbSet<Entities.Faq> Faqs { get; set; }

    public DbSet<FaqItem> FaqItems { get; set; }

    protected override string ConfigurationNamespace =>
        "BaseFaq.Faq.Persistence.FaqDb.Configurations";

    protected override IEnumerable<string> ConfigurationNamespaces =>
    [
        ConfigurationNamespace,
        "BaseFaq.Common.EntityFramework.Core.Configurations"
    ];
}