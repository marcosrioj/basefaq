using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Tools.Seed.Configuration;

namespace BaseFaq.Tools.Seed.Abstractions;

public interface ITenantSeedService
{
    bool HasData(TenantDbContext dbContext);
    Guid Seed(TenantDbContext dbContext, TenantSeedRequest request, SeedCounts counts);
}