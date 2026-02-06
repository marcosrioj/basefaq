using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Tenant.Dtos.Tenant;
using BaseFaq.Tenant.TenantWeb.Business.Abstractions;
using BaseFaq.Tenant.TenantWeb.Business.Commands.CreateTenant;
using BaseFaq.Tenant.TenantWeb.Business.Commands.SetDefaultTenant;
using BaseFaq.Tenant.TenantWeb.Business.Commands.UpdateTenant;
using BaseFaq.Tenant.TenantWeb.Business.Queries.GetTenant;
using BaseFaq.Tenant.TenantWeb.Business.Queries.GetTenantList;
using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Service;

public class TenantService(IMediator mediator) : ITenantService
{
    public async Task<TenantDto> Create(TenantCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new TenantsCreateTenantCommand
        {
            Slug = requestDto.Slug,
            Name = requestDto.Name,
            Edition = requestDto.Edition,
            App = requestDto.App,
            ConnectionString = requestDto.ConnectionString,
            IsActive = requestDto.IsActive
        };

        var id = await mediator.Send(command, token);

        var result = await mediator.Send(new TenantsGetTenantQuery { Id = id }, token);
        if (result is null)
        {
            throw new InvalidOperationException($"Created tenant '{id}' was not found.");
        }

        return result;
    }

    public Task<PagedResultDto<TenantDto>> GetAll(TenantGetAllRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new TenantsGetTenantListQuery { Request = requestDto }, token);
    }

    public async Task<TenantDto> GetById(Guid id, CancellationToken token)
    {
        var result = await mediator.Send(new TenantsGetTenantQuery { Id = id }, token);
        if (result is null)
        {
            throw new KeyNotFoundException($"Tenant '{id}' was not found.");
        }

        return result;
    }

    public Task<bool> SetDefault(CancellationToken token)
    {
        return mediator.Send(new TenantsSetDefaultTenantCommand(), token);
    }

    public async Task<TenantDto> Update(Guid id, TenantUpdateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new TenantsUpdateTenantCommand
        {
            Id = id,
            Slug = requestDto.Slug,
            Name = requestDto.Name,
            Edition = requestDto.Edition,
            ConnectionString = requestDto.ConnectionString,
            IsActive = requestDto.IsActive
        };

        await mediator.Send(command, token);

        return await GetById(id, token);
    }
}