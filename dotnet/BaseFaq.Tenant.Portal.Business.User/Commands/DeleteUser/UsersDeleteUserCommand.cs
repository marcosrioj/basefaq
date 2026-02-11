using MediatR;

namespace BaseFaq.Tenant.Portal.Business.User.Commands.DeleteUser;

public class UsersDeleteUserCommand : IRequest
{
    public required Guid Id { get; set; }
}