using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Tenant.Dtos.User;
using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Queries.GetUserList;

public class UsersGetUserListQuery : IRequest<PagedResultDto<UserDto>>
{
    public required UserGetAllRequestDto Request { get; set; }
}