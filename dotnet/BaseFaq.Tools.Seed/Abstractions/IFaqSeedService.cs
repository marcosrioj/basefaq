using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Tools.Seed.Configuration;

namespace BaseFaq.Tools.Seed.Abstractions;

public interface IFaqSeedService
{
    bool HasData(FaqDbContext dbContext);
    void Seed(FaqDbContext dbContext, Guid tenantId, SeedCounts counts);
}