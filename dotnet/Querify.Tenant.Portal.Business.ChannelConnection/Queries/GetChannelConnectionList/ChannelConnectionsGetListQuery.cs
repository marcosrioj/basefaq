using MediatR;
using Querify.Models.Common.Dtos;
using Querify.Models.Tenant.Dtos.ChannelConnection;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Queries.GetChannelConnectionList;

public sealed class ChannelConnectionsGetListQuery : IRequest<PagedResultDto<ChannelConnectionDto>>
{
    public required Guid TenantId { get; set; }
    public required ChannelConnectionGetAllRequestDto Request { get; set; }
}
