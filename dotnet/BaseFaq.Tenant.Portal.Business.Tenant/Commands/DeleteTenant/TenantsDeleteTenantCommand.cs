using MediatR;

namespace BaseFaq.Tenant.Portal.Business.Tenant.Commands.DeleteTenant;

public class TenantsDeleteTenantCommand : IRequest
{
    public required Guid Id { get; set; }
}