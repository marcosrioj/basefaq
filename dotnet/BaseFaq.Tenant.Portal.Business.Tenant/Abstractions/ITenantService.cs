using BaseFaq.Tenant.Portal.Business.Tenant.Dtos;

namespace BaseFaq.Tenant.Portal.Business.Tenant.Abstractions;

public interface ITenantService
{
    Task<List<TenantSummaryDto>> GetAll(CancellationToken token);
    Task<List<TenantSummaryDto>> CreateOrUpdate(TenantCreateOrUpdateRequestDto requestDto, CancellationToken token);
}