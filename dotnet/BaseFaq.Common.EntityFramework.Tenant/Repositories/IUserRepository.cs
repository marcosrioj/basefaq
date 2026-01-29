using BaseFaq.Common.EntityFramework.Core.Repositories;

namespace BaseFaq.Common.EntityFramework.Tenant.Repositories;

public interface IUserRepository : IBaseRepository<Entities.User>
{
}