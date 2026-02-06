using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Tenant.Dtos.TenantConnection;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Tenant.TenantWeb.Business.Abstractions;
using BaseFaq.Tenant.TenantWeb.Business.Commands.CreateTenantConnection;
using BaseFaq.Tenant.TenantWeb.Business.Commands.UpdateTenantConnection;
using BaseFaq.Tenant.TenantWeb.Business.Queries.GetTenantConnection;
using BaseFaq.Tenant.TenantWeb.Business.Queries.GetTenantConnectionList;
using MediatR;
using System.Net;

namespace BaseFaq.Tenant.TenantWeb.Business.Service;

public class TenantConnectionService(IMediator mediator) : ITenantConnectionService
{
    public async Task<TenantConnectionDto> Create(TenantConnectionCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new TenantConnectionsCreateTenantConnectionCommand
        {
            App = requestDto.App,
            ConnectionString = requestDto.ConnectionString,
            IsCurrent = requestDto.IsCurrent
        };

        var id = await mediator.Send(command, token);

        var result = await mediator.Send(new TenantConnectionsGetTenantConnectionQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"Created tenant connection '{id}' was not found.",
                errorCode: (int)HttpStatusCode.InternalServerError);
        }

        return result;
    }

    public Task<PagedResultDto<TenantConnectionDto>> GetAll(TenantConnectionGetAllRequestDto requestDto,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new TenantConnectionsGetTenantConnectionListQuery { Request = requestDto }, token);
    }

    public async Task<TenantConnectionDto> GetById(Guid id, CancellationToken token)
    {
        var result = await mediator.Send(new TenantConnectionsGetTenantConnectionQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"Tenant connection '{id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        return result;
    }

    public async Task<TenantConnectionDto> Update(Guid id, TenantConnectionUpdateRequestDto requestDto,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new TenantConnectionsUpdateTenantConnectionCommand
        {
            Id = id,
            App = requestDto.App,
            ConnectionString = requestDto.ConnectionString,
            IsCurrent = requestDto.IsCurrent
        };

        await mediator.Send(command, token);

        return await GetById(id, token);
    }
}