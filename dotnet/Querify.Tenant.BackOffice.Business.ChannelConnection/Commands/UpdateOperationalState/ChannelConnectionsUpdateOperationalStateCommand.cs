using MediatR;
using Querify.Models.Tenant.Dtos.ChannelConnection;

namespace Querify.Tenant.BackOffice.Business.ChannelConnection.Commands.UpdateOperationalState;

public sealed class ChannelConnectionsUpdateOperationalStateCommand : IRequest<Guid>
{
    public required Guid Id { get; set; }
    public required ChannelConnectionOperationalUpdateRequestDto Request { get; set; }
}
