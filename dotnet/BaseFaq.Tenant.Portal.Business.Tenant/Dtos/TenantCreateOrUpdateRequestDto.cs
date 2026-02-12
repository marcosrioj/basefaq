using BaseFaq.Models.Tenant.Enums;

namespace BaseFaq.Tenant.Portal.Business.Tenant.Dtos;

public class TenantCreateOrUpdateRequestDto
{
    public required string Name { get; set; }
    public required TenantEdition Edition { get; set; }
}