using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Tenant.Commands.DeleteTenant;

public class TenantsDeleteTenantCommand : IRequest
{
    public required Guid Id { get; set; }
}