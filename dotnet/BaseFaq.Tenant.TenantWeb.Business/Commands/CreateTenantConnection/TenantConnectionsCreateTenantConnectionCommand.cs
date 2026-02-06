using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Commands.CreateTenantConnection;

public class TenantConnectionsCreateTenantConnectionCommand : IRequest<Guid>
{
    public required Guid TenantId { get; set; }
    public required string ConnectionString { get; set; }
    public required bool IsCurrent { get; set; }
}