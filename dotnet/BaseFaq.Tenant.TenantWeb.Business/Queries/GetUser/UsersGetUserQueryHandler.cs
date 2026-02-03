using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Models.Tenant.Dtos.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Tenant.TenantWeb.Business.Queries.GetUser;

public class UsersGetUserQueryHandler(TenantDbContext dbContext)
    : IRequestHandler<UsersGetUserQuery, UserDto?>
{
    public async Task<UserDto?> Handle(UsersGetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);

        if (user is null)
        {
            return null;
        }

        return new UserDto
        {
            Id = user.Id,
            GivenName = user.GivenName,
            SurName = user.SurName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            TenantId = user.TenantId
        };
    }
}