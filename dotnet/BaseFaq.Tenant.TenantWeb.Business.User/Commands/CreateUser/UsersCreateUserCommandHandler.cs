using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Common.EntityFramework.Tenant.Entities;
using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.User.Commands.CreateUser;

public class UsersCreateUserCommandHandler(TenantDbContext dbContext)
    : IRequestHandler<UsersCreateUserCommand, Guid>
{
    public async Task<Guid> Handle(UsersCreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new Common.EntityFramework.Tenant.Entities.User
        {
            GivenName = request.GivenName,
            SurName = request.SurName,
            Email = request.Email,
            ExternalId = request.ExternalId,
            PhoneNumber = request.PhoneNumber ?? string.Empty,
            Role = request.Role
        };

        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}