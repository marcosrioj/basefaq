using BaseFaq.Models.Tenant.Dtos.Tenant;
using BaseFaq.Tenant.Portal.Business.Tenant.Abstractions;
using BaseFaq.Tenant.Portal.Business.Tenant.Commands.CreateOrUpdateTenants;
using BaseFaq.Tenant.Portal.Business.Tenant.Queries.GetAllTenants;
using MediatR;

namespace BaseFaq.Tenant.Portal.Business.Tenant.Service;

public class TenantService(IMediator mediator) : ITenantService
{
    public Task<List<TenantSummaryDto>> GetAll(CancellationToken token)
    {
        return mediator.Send(new TenantsGetAllTenantsQuery(), token);
    }

    public Task<List<TenantSummaryDto>> CreateOrUpdate(TenantCreateOrUpdateRequestDto requestDto,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new TenantsCreateOrUpdateTenantsCommand
        {
            Name = requestDto.Name,
            Edition = requestDto.Edition
        };

        return mediator.Send(command, token);
    }
}