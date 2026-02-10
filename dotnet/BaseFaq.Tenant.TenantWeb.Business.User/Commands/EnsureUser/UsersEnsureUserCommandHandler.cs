using System.Net;
using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.User.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Tenant.TenantWeb.Business.User.Commands.EnsureUser;

public class UsersEnsureUserCommandHandler(
    TenantDbContext dbContext,
    IClaimService claimService)
    : IRequestHandler<UsersEnsureUserCommand, Guid>
{
    public async Task<Guid> Handle(UsersEnsureUserCommand request,
        CancellationToken cancellationToken)
    {
        var externalUserId = claimService.GetExternalUserId();
        var userName = claimService.GetName();
        var email = claimService.GetEmail();

        if (userName is null || email is null || externalUserId is null)
        {
            throw new ApiErrorException(
                $"User: '{externalUserId}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.ExternalId == externalUserId, cancellationToken);

        if (user is not null)
        {
            return user.Id;
        }

        user = new Common.EntityFramework.Tenant.Entities.User
        {
            Id = Guid.NewGuid(),
            GivenName = userName,
            ExternalId = externalUserId,
            Email = email,
            Role = UserRoleType.Member
        };

        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}