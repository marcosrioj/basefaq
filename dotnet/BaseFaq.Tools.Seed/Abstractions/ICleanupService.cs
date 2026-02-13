using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Faq.Common.Persistence.FaqDb;

namespace BaseFaq.Tools.Seed.Abstractions;

public interface ICleanupService
{
    void CleanTenantDb(TenantDbContext dbContext);
    void CleanFaqDb(FaqDbContext dbContext);
}