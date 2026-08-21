using MediatR;
using Querify.Models.Common.Dtos;
using Querify.Models.Tenant.Dtos.ChannelConnection;
using Querify.Tenant.BackOffice.Business.ChannelConnection.Abstractions;
using Querify.Tenant.BackOffice.Business.ChannelConnection.Commands.UpdateOperationalState;
using Querify.Tenant.BackOffice.Business.ChannelConnection.Queries.GetChannelConnection;
using Querify.Tenant.BackOffice.Business.ChannelConnection.Queries.GetChannelConnectionList;

namespace Querify.Tenant.BackOffice.Business.ChannelConnection.Service;

public sealed class ChannelConnectionBackOfficeService(IMediator mediator) : IChannelConnectionBackOfficeService
{
    public Task<ChannelConnectionDto> GetById(Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new ChannelConnectionsGetQuery { Id = id }, cancellationToken);

    public Task<PagedResultDto<ChannelConnectionDto>> GetAll(
        ChannelConnectionGetAllRequestDto request,
        CancellationToken cancellationToken) =>
        mediator.Send(new ChannelConnectionsGetListQuery { Request = request }, cancellationToken);

    public Task<Guid> UpdateOperationalState(
        Guid id,
        ChannelConnectionOperationalUpdateRequestDto request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new ChannelConnectionsUpdateOperationalStateCommand { Id = id, Request = request },
            cancellationToken);
}
