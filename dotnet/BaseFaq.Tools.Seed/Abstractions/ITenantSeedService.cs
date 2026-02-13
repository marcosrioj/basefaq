using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Tools.Seed.Configuration;

namespace BaseFaq.Tools.Seed.Abstractions;

public interface ITenantSeedService
{
    bool HasData(TenantDbContext dbContext);
    Guid SeedDummyData(TenantDbContext dbContext, TenantSeedRequest request, SeedCounts counts);
    Guid EnsureIaAgentUser(TenantDbContext dbContext);
}