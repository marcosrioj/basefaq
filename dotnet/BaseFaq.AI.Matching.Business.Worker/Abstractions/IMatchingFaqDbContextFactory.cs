using BaseFaq.Faq.Common.Persistence.FaqDb;

namespace BaseFaq.AI.Matching.Business.Worker.Abstractions;

public interface IMatchingFaqDbContextFactory
{
    FaqDbContext Create(Guid tenantId);
}