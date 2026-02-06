using BaseFaq.Models.Tenant.Dtos.Tenant;
using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Commands.SetDefaultTenant;

public class TenantsSetDefaultTenantCommand : IRequest<TenantDto>
{
    public Guid? TenantId { get; set; }
}