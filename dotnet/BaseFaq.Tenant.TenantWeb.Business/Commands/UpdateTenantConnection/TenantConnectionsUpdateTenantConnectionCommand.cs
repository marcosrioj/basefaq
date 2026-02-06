using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Commands.UpdateTenantConnection;

public class TenantConnectionsUpdateTenantConnectionCommand : IRequest
{
    public required Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required string ConnectionString { get; set; }
    public required bool IsCurrent { get; set; }
}