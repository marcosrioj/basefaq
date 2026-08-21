using Querify.Common.EntityFramework.Tenant;
using Querify.Broadcast.Common.Persistence.BroadcastDb.DbContext;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.QnA.Common.Persistence.QnADb.DbContext;

namespace Querify.Tools.Seed.Abstractions;

public interface ICleanupService
{
    void CleanTenantDb(TenantDbContext dbContext);
    void CleanQnADb(QnADbContext dbContext);
    void CleanDirectDb(DirectDbContext dbContext);
    void CleanBroadcastDb(BroadcastDbContext dbContext);
    void CleanBigDataQnADb(QnADbContext dbContext);
}
