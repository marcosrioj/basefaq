using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Core.Entities;

namespace BaseFaq.Common.EntityFramework.Tenant.Entities;

public class TenantConnection : BaseEntity, IMustHaveTenant
{
    public const int MaxConnectionStringLength = 1024;

    public required Guid TenantId { get; set; }
    public required string ConnectionString { get; set; }
    public required bool IsCurrent { get; set; }
}