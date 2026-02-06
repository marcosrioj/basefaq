using BaseFaq.Common.EntityFramework.Core.Entities;
using BaseFaq.Models.Common.Enums;

namespace BaseFaq.Common.EntityFramework.Tenant.Entities;

public class TenantConnection : BaseEntity
{
    public const int MaxConnectionStringLength = 1024;

    public required string ConnectionString { get; set; }
    public required AppEnum App { get; set; }
    public required bool IsCurrent { get; set; }
}