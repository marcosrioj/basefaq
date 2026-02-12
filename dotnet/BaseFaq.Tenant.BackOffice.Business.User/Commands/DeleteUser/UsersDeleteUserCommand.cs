using MediatR;

namespace BaseFaq.Tenant.BackOffice.Business.User.Commands.DeleteUser;

public class UsersDeleteUserCommand : IRequest
{
    public required Guid Id { get; set; }
}