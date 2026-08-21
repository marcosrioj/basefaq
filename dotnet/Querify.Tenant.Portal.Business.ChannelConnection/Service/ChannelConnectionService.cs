using MediatR;
using Querify.Models.Common.Dtos;
using Querify.Models.Tenant.Dtos.ChannelConnection;
using Querify.Tenant.Portal.Business.ChannelConnection.Abstractions;
using Querify.Tenant.Portal.Business.ChannelConnection.Commands.CreateChannelConnection;
using Querify.Tenant.Portal.Business.ChannelConnection.Commands.DeleteChannelConnection;
using Querify.Tenant.Portal.Business.ChannelConnection.Commands.UpdateChannelConnection;
using Querify.Tenant.Portal.Business.ChannelConnection.Queries.GetChannelConnection;
using Querify.Tenant.Portal.Business.ChannelConnection.Queries.GetChannelConnectionList;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Service;

public sealed class ChannelConnectionService(IMediator mediator) : IChannelConnectionService
{
    public Task<Guid> Create(
        Guid tenantId,
        ChannelConnectionCreateRequestDto request,
        CancellationToken cancellationToken) =>
        mediator.Send(new ChannelConnectionsCreateCommand { TenantId = tenantId, Request = request }, cancellationToken);

    public Task<Guid> Update(
        Guid tenantId,
        Guid id,
        ChannelConnectionUpdateRequestDto request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new ChannelConnectionsUpdateCommand { TenantId = tenantId, Id = id, Request = request },
            cancellationToken);

    public Task Delete(Guid tenantId, Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new ChannelConnectionsDeleteCommand { TenantId = tenantId, Id = id }, cancellationToken);

    public Task<ChannelConnectionDto> GetById(Guid tenantId, Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new ChannelConnectionsGetQuery { TenantId = tenantId, Id = id }, cancellationToken);

    public Task<PagedResultDto<ChannelConnectionDto>> GetAll(
        Guid tenantId,
        ChannelConnectionGetAllRequestDto request,
        CancellationToken cancellationToken) =>
        mediator.Send(new ChannelConnectionsGetListQuery { TenantId = tenantId, Request = request }, cancellationToken);
}
