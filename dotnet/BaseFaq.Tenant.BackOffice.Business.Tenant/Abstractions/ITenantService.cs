using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Tenant.Dtos.Tenant;

namespace BaseFaq.Tenant.BackOffice.Business.Tenant.Abstractions;

public interface ITenantService
{
    Task<TenantDto> Create(TenantCreateRequestDto requestDto, CancellationToken token);
    Task Delete(Guid id, CancellationToken token);
    Task<PagedResultDto<TenantDto>> GetAll(TenantGetAllRequestDto requestDto, CancellationToken token);
    Task<TenantDto> GetById(Guid id, CancellationToken token);
    Task<TenantDto> Update(Guid id, TenantUpdateRequestDto requestDto, CancellationToken token);
}