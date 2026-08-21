using MediatR;
using Querify.Models.Tenant.Dtos.ChannelConnection;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Commands.CreateChannelConnection;

public sealed class ChannelConnectionsCreateCommand : IRequest<Guid>
{
    public required Guid TenantId { get; set; }
    public required ChannelConnectionCreateRequestDto Request { get; set; }
}
