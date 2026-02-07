using BaseFaq.Models.Tenant.Dtos.Tenant;
using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Tenant.Queries.GetTenant;

public class TenantsGetTenantQuery : IRequest<TenantDto?>
{
    public required Guid Id { get; set; }
}