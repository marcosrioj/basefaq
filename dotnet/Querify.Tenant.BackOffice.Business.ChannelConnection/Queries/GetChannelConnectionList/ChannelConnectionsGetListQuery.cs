using MediatR;
using Querify.Models.Common.Dtos;
using Querify.Models.Tenant.Dtos.ChannelConnection;

namespace Querify.Tenant.BackOffice.Business.ChannelConnection.Queries.GetChannelConnectionList;

public sealed class ChannelConnectionsGetListQuery : IRequest<PagedResultDto<ChannelConnectionDto>>
{
    public required ChannelConnectionGetAllRequestDto Request { get; set; }
}
