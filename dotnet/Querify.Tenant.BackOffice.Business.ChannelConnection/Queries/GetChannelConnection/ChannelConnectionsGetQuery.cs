using MediatR;
using Querify.Models.Tenant.Dtos.ChannelConnection;

namespace Querify.Tenant.BackOffice.Business.ChannelConnection.Queries.GetChannelConnection;

public sealed class ChannelConnectionsGetQuery : IRequest<ChannelConnectionDto>
{
    public required Guid Id { get; set; }
}
