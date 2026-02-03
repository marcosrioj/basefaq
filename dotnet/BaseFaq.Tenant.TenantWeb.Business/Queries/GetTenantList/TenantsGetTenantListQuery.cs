using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Tenant.Dtos.Tenant;
using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Queries.GetTenantList;

public class TenantsGetTenantListQuery : IRequest<PagedResultDto<TenantDto>>
{
    public required TenantGetAllRequestDto Request { get; set; }
}