using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Tenant.Dtos.Tenant;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Tenant.TenantWeb.Business.Tenant.Abstractions;
using BaseFaq.Tenant.TenantWeb.Business.Tenant.Commands.CreateTenant;
using BaseFaq.Tenant.TenantWeb.Business.Tenant.Commands.DeleteTenant;
using BaseFaq.Tenant.TenantWeb.Business.Tenant.Commands.UpdateTenant;
using BaseFaq.Tenant.TenantWeb.Business.Tenant.Queries.GetTenant;
using BaseFaq.Tenant.TenantWeb.Business.Tenant.Queries.GetTenantList;
using MediatR;
using System.Net;

namespace BaseFaq.Tenant.TenantWeb.Business.Tenant.Service;

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
            throw new ApiErrorException(
                $"Created tenant '{id}' was not found.",
                errorCode: (int)HttpStatusCode.InternalServerError);
        }

        return result;
    }

    public Task<PagedResultDto<TenantDto>> GetAll(TenantGetAllRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new TenantsGetTenantListQuery { Request = requestDto }, token);
    }

    public Task Delete(Guid id, CancellationToken token)
    {
        return mediator.Send(new TenantsDeleteTenantCommand { Id = id }, token);
    }

    public async Task<TenantDto> GetById(Guid id, CancellationToken token)
    {
        var result = await mediator.Send(new TenantsGetTenantQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"Tenant '{id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        return result;
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