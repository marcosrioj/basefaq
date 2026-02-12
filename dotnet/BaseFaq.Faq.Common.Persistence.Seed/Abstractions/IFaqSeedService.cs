using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Faq.Common.Persistence.Seed.Configuration;

namespace BaseFaq.Faq.Common.Persistence.Seed.Abstractions;

public interface IFaqSeedService
{
    bool HasData(FaqDbContext dbContext);
    void Seed(FaqDbContext dbContext, Guid tenantId, SeedCounts counts);
}