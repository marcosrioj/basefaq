using BaseFaq.Models.Tenant.Dtos.User;
using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Queries.GetUser;

public class UsersGetUserQuery : IRequest<UserDto?>
{
    public required Guid Id { get; set; }
}