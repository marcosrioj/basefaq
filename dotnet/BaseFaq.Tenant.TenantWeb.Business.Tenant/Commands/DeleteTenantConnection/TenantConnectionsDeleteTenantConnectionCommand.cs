using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Tenant.Commands.DeleteTenantConnection;

public class TenantConnectionsDeleteTenantConnectionCommand : IRequest
{
    public required Guid Id { get; set; }
}