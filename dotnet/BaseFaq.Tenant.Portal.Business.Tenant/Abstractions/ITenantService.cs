using BaseFaq.Models.Tenant.Dtos.Tenant;

namespace BaseFaq.Tenant.Portal.Business.Tenant.Abstractions;

public interface ITenantService
{
    Task<List<TenantSummaryDto>> GetAll(CancellationToken token);
    Task<List<TenantSummaryDto>> CreateOrUpdate(TenantCreateOrUpdateRequestDto requestDto, CancellationToken token);
}