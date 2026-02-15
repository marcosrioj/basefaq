using System.Net;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Common.Enums;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.User.Dtos.User;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using MediatR;
using BaseFaq.Tenant.BackOffice.Business.User.Abstractions;
using BaseFaq.Tenant.BackOffice.Business.User.Commands.CreateUser;
using BaseFaq.Tenant.BackOffice.Business.User.Commands.DeleteUser;
using BaseFaq.Tenant.BackOffice.Business.User.Commands.UpdateUser;
using BaseFaq.Tenant.BackOffice.Business.User.Queries.GetUser;
using BaseFaq.Tenant.BackOffice.Business.User.Queries.GetUserList;

namespace BaseFaq.Tenant.BackOffice.Business.User.Service;

public class UserService(IMediator mediator) : IUserService
{
    public async Task<Guid> Create(UserCreateRequestDto requestDto, CancellationToken token)
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

        return await mediator.Send(command, token);
    }

    public Task<PagedResultDto<UserDto>> GetAll(UserGetAllRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new UsersGetUserListQuery { Request = requestDto }, token);
    }

    public Task Delete(Guid id, CancellationToken token)
    {
        return mediator.Send(new UsersDeleteUserCommand { Id = id }, token);
    }

    public async Task<UserDto> GetById(Guid id, CancellationToken token)
    {
        var result = await mediator.Send(new UsersGetUserQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"User '{id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        return result;
    }

    public async Task<Guid> Update(Guid id, UserUpdateRequestDto requestDto, CancellationToken token)
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
        return id;
    }
}