using BaseFaq.Tenant.Portal.Business.Tenant.Dtos;
using MediatR;

namespace BaseFaq.Tenant.Portal.Business.Tenant.Queries.GetAllTenants;

public class TenantsGetAllTenantsQuery : IRequest<List<TenantSummaryDto>>;