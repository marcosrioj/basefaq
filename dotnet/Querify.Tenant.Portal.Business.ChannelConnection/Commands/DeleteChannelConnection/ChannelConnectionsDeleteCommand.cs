using MediatR;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Commands.DeleteChannelConnection;

public sealed class ChannelConnectionsDeleteCommand : IRequest
{
    public required Guid TenantId { get; set; }
    public required Guid Id { get; set; }
}
