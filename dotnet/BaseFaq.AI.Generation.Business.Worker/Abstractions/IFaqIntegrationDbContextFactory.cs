using BaseFaq.Faq.Common.Persistence.FaqDb;

namespace BaseFaq.AI.Generation.Business.Worker.Abstractions;

public interface IFaqIntegrationDbContextFactory
{
    FaqDbContext Create(Guid tenantId);
}