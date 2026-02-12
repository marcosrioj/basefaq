using MediatR;

namespace BaseFaq.Tenant.BackOffice.Business.Tenant.Commands.DeleteTenant;

public class TenantsDeleteTenantCommand : IRequest
{
    public required Guid Id { get; set; }
}