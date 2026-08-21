using MediatR;
using Querify.Models.Tenant.Dtos.ChannelConnection;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Queries.GetChannelConnection;

public sealed class ChannelConnectionsGetQuery : IRequest<ChannelConnectionDto>
{
    public required Guid TenantId { get; set; }
    public required Guid Id { get; set; }
}
