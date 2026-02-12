using BaseFaq.Models.Tenant.Dtos.Tenant;
using BaseFaq.Models.Tenant.Enums;
using MediatR;

namespace BaseFaq.Tenant.Portal.Business.Tenant.Commands.CreateOrUpdateTenants;

public class TenantsCreateOrUpdateTenantsCommand : IRequest<List<TenantSummaryDto>>
{
    public required string Name { get; set; }
    public required TenantEdition Edition { get; set; }
}