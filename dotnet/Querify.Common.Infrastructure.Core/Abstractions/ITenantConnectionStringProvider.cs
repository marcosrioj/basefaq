using Querify.Models.Common.Enums;

namespace Querify.Common.Infrastructure.Core.Abstractions;

public interface ITenantConnectionStringProvider
{
    string GetConnectionString(Guid tenantId, ModuleEnum module);
}
