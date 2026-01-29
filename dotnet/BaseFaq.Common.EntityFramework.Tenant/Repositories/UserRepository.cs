using BaseFaq.Common.EntityFramework.Core.Repositories;

namespace BaseFaq.Common.EntityFramework.Tenant.Repositories;

public sealed class UserRepository : BaseRepository<TenantDbContext, Entities.User>, IUserRepository
{
    public UserRepository(TenantDbContext context)
        : base(context)
    {
    }
}