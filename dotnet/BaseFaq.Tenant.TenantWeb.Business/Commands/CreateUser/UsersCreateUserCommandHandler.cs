using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Common.EntityFramework.Tenant.Entities;
using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Commands.CreateUser;

public class UsersCreateUserCommandHandler(TenantDbContext dbContext)
    : IRequestHandler<UsersCreateUserCommand, Guid>
{
    public async Task<Guid> Handle(UsersCreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            GivenName = request.GivenName,
            SurName = request.SurName,
            Email = request.Email,
            ExternalId = request.ExternalId,
            PhoneNumber = request.PhoneNumber ?? string.Empty,
            Role = request.Role,
            TenantId = request.TenantId
        };

        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}