using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Tenant.Dtos.TenantConnection;
using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Tenant.Queries.GetTenantConnectionList;

public class TenantConnectionsGetTenantConnectionListQuery : IRequest<PagedResultDto<TenantConnectionDto>>
{
    public required TenantConnectionGetAllRequestDto Request { get; set; }
}