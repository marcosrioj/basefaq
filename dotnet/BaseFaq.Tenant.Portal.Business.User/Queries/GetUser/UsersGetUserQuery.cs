using BaseFaq.Models.User.Dtos.User;
using MediatR;

namespace BaseFaq.Tenant.Portal.Business.User.Queries.GetUser;

public class UsersGetUserQuery : IRequest<UserDto?>
{
    public required Guid Id { get; set; }
}