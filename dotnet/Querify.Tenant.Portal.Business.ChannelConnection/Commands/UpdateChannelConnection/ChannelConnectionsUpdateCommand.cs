using MediatR;
using Querify.Models.Tenant.Dtos.ChannelConnection;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Commands.UpdateChannelConnection;

public sealed class ChannelConnectionsUpdateCommand : IRequest<Guid>
{
    public required Guid TenantId { get; set; }
    public required Guid Id { get; set; }
    public required ChannelConnectionUpdateRequestDto Request { get; set; }
}
