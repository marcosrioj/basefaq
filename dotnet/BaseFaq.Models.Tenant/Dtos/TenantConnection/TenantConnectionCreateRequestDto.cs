using BaseFaq.Models.Common.Enums;

namespace BaseFaq.Models.Tenant.Dtos.TenantConnection;

public class TenantConnectionCreateRequestDto
{
    public required AppEnum App { get; set; }
    public required string ConnectionString { get; set; }
    public required bool IsCurrent { get; set; }
}