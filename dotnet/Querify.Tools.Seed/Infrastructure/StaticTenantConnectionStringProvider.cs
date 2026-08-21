using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Models.Common.Enums;

namespace Querify.Tools.Seed.Infrastructure;

public sealed class StaticTenantConnectionStringProvider(string connectionString) : ITenantConnectionStringProvider
{
    public string GetConnectionString(Guid tenantId, ModuleEnum module)
    {
        return connectionString;
    }
}
