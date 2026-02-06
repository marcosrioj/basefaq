using BaseFaq.Models.Tenant.Dtos.TenantConnection;
using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Queries.GetTenantConnection;

public class TenantConnectionsGetTenantConnectionQuery : IRequest<TenantConnectionDto?>
{
    public required Guid Id { get; set; }
}