using Querify.Models.Common.Dtos;
using Querify.Models.Tenant.Dtos.ChannelConnection;

namespace Querify.Tenant.BackOffice.Business.ChannelConnection.Abstractions;

public interface IChannelConnectionBackOfficeService
{
    Task<ChannelConnectionDto> GetById(Guid id, CancellationToken cancellationToken);
    Task<PagedResultDto<ChannelConnectionDto>> GetAll(
        ChannelConnectionGetAllRequestDto request,
        CancellationToken cancellationToken);
    Task<Guid> UpdateOperationalState(
        Guid id,
        ChannelConnectionOperationalUpdateRequestDto request,
        CancellationToken cancellationToken);
}
