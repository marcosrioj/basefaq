using Querify.Models.Common.Dtos;
using Querify.Models.Tenant.Dtos.ChannelConnection;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Abstractions;

public interface IChannelConnectionService
{
    Task<Guid> Create(Guid tenantId, ChannelConnectionCreateRequestDto request, CancellationToken cancellationToken);
    Task<Guid> Update(Guid tenantId, Guid id, ChannelConnectionUpdateRequestDto request, CancellationToken cancellationToken);
    Task Delete(Guid tenantId, Guid id, CancellationToken cancellationToken);
    Task<ChannelConnectionDto> GetById(Guid tenantId, Guid id, CancellationToken cancellationToken);
    Task<PagedResultDto<ChannelConnectionDto>> GetAll(
        Guid tenantId,
        ChannelConnectionGetAllRequestDto request,
        CancellationToken cancellationToken);
}
