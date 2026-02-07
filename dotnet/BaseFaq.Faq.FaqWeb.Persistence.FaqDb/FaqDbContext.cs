using BaseFaq.Common.EntityFramework.Core;
using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using BaseFaq.Models.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb;

public class FaqDbContext(
    DbContextOptions<FaqDbContext> options,
    ISessionService sessionService,
    IConfiguration configuration,
    ITenantConnectionStringProvider tenantConnectionStringProvider)
    : BaseDbContext<FaqDbContext>(
        options,
        sessionService,
        configuration,
        tenantConnectionStringProvider)
{
    public DbSet<Entities.Faq> Faqs { get; set; }

    public DbSet<FaqItem> FaqItems { get; set; }

    protected override IEnumerable<string> ConfigurationNamespaces =>
    [
        "BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Configurations"
    ];

    protected override AppEnum SessionApp => AppEnum.FaqWeb;
}