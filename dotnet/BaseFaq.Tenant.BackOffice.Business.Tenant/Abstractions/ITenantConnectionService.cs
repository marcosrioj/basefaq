using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Tenant.Dtos.TenantConnection;

namespace BaseFaq.Tenant.BackOffice.Business.Tenant.Abstractions;

public interface ITenantConnectionService
{
    Task<TenantConnectionDto> Create(TenantConnectionCreateRequestDto requestDto, CancellationToken token);

    Task Delete(Guid id, CancellationToken token);

    Task<PagedResultDto<TenantConnectionDto>> GetAll(TenantConnectionGetAllRequestDto requestDto,
        CancellationToken token);

    Task<TenantConnectionDto> GetById(Guid id, CancellationToken token);
    Task<TenantConnectionDto> Update(Guid id, TenantConnectionUpdateRequestDto requestDto, CancellationToken token);
}