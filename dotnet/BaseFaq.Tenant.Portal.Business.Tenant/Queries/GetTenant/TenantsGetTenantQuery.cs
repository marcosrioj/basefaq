using BaseFaq.Models.Tenant.Dtos.Tenant;
using MediatR;

namespace BaseFaq.Tenant.Portal.Business.Tenant.Queries.GetTenant;

public class TenantsGetTenantQuery : IRequest<TenantDto?>
{
    public required Guid Id { get; set; }
}