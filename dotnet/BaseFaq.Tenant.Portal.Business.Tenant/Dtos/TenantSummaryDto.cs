using BaseFaq.Models.Common.Enums;
using BaseFaq.Models.Tenant.Enums;

namespace BaseFaq.Tenant.Portal.Business.Tenant.Dtos;

public class TenantSummaryDto
{
    public required Guid Id { get; set; }
    public required string Slug { get; set; }
    public required string Name { get; set; }
    public required TenantEdition Edition { get; set; }
    public required AppEnum App { get; set; }
    public required bool IsActive { get; set; }
}