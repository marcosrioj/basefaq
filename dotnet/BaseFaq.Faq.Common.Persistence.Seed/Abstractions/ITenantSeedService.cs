using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Faq.Common.Persistence.Seed.Configuration;

namespace BaseFaq.Faq.Common.Persistence.Seed.Abstractions;

public interface ITenantSeedService
{
    bool HasData(TenantDbContext dbContext);
    Guid Seed(TenantDbContext dbContext, TenantSeedRequest request, SeedCounts counts);
}