using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Common.Enums;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Tenant.Dtos.User;
using BaseFaq.Tenant.TenantWeb.Business.Abstractions;
using BaseFaq.Tenant.TenantWeb.Business.Commands.CreateUser;
using BaseFaq.Tenant.TenantWeb.Business.Commands.UpdateUser;
using BaseFaq.Tenant.TenantWeb.Business.Queries.GetUser;
using BaseFaq.Tenant.TenantWeb.Business.Queries.GetUserList;
using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Service;

public class UserService(IMediator mediator) : IUserService
{
    public async Task<UserDto> Create(UserCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new UsersCreateUserCommand
        {
            GivenName = requestDto.GivenName,
            SurName = requestDto.SurName,
            Email = requestDto.Email,
            ExternalId = requestDto.ExternalId,
            PhoneNumber = requestDto.PhoneNumber,
            Role = requestDto.Role
        };

        var id = await mediator.Send(command, token);

        var result = await mediator.Send(new UsersGetUserQuery { Id = id }, token);
        if (result is null)
        {
            throw new InvalidOperationException($"Created user '{id}' was not found.");
        }

        return result;
    }

    public Task<PagedResultDto<UserDto>> GetAll(UserGetAllRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new UsersGetUserListQuery { Request = requestDto }, token);
    }

    public async Task<UserDto> GetById(Guid id, CancellationToken token)
    {
        var result = await mediator.Send(new UsersGetUserQuery { Id = id }, token);
        if (result is null)
        {
            throw new KeyNotFoundException($"User '{id}' was not found.");
        }

        return result;
    }

    public async Task<UserDto> Update(Guid id, UserUpdateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new UsersUpdateUserCommand
        {
            Id = id,
            GivenName = requestDto.GivenName,
            SurName = requestDto.SurName,
            Email = requestDto.Email,
            ExternalId = requestDto.ExternalId,
            PhoneNumber = requestDto.PhoneNumber,
            Role = requestDto.Role
        };

        await mediator.Send(command, token);

        return await GetById(id, token);
    }
}