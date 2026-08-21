using System.Net;
using Querify.Common.EntityFramework.Tenant;
using Querify.Common.EntityFramework.Tenant.Entities;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Common.Infrastructure.Core.Helpers;
using Querify.Models.Tenant.Enums;
using Querify.Tenant.Portal.Business.Tenant.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Querify.Tenant.Portal.Business.Tenant.Commands.AddTenantMember;

public class TenantUsersAddTenantMemberCommandHandler(
    TenantDbContext dbContext,
    IAllowedTenantStore allowedTenantStore,
    ITenantPortalAccessService tenantPortalAccessService,
    ISessionService sessionService)
    : IRequestHandler<TenantUsersAddTenantMemberCommand, Guid>
{
    public async Task<Guid> Handle(TenantUsersAddTenantMemberCommand request, CancellationToken cancellationToken)
    {
        var tenantId = request.TenantId;
        var currentUserId = sessionService.GetUserId();
        await tenantPortalAccessService.EnsureOwnerAccessAsync(tenantId, cancellationToken);
        var workspaceId = await dbContext.Tenants
            .Where(entity => entity.Id == tenantId)
            .Select(entity => entity.WorkspaceId)
            .SingleAsync(cancellationToken);

        if (request.Role != TenantUserRoleType.Member)
        {
            throw new ApiErrorException(
                "Portal users can only add members to the workspace.",
                errorCode: (int)HttpStatusCode.BadRequest);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var trimmedName = request.Name.Trim();
        var existingTenantUser = await dbContext.TenantUsers
            .Include(entity => entity.User)
            .FirstOrDefaultAsync(
                entity => entity.TenantId == tenantId && entity.User.Email.ToLower() == normalizedEmail,
                cancellationToken);

        if (existingTenantUser is not null)
        {
            if (existingTenantUser.UserId != currentUserId)
            {
                throw CreateExistingTenantUserEmailException();
            }

            existingTenantUser.User.GivenName = trimmedName;

            var selectedMembershipId = await EnsureWorkspaceMembershipsAsync(
                workspaceId,
                tenantId,
                existingTenantUser.UserId,
                request.Role,
                cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            await AllowedTenantCacheHelper.RemoveUserEntries(
                allowedTenantStore,
                [existingTenantUser.UserId],
                cancellationToken);

            return selectedMembershipId;
        }

        var user = await ResolveOrEnsureUserByEmailAsync(normalizedEmail, trimmedName, cancellationToken);
        if (user.Id == currentUserId)
        {
            user.GivenName = trimmedName;
        }

        var existingMembershipForUser = await dbContext.TenantUsers
            .FirstOrDefaultAsync(
                entity => entity.TenantId == tenantId && entity.UserId == user.Id,
                cancellationToken);

        if (existingMembershipForUser is not null)
        {
            var selectedMembershipId = await EnsureWorkspaceMembershipsAsync(
                workspaceId,
                tenantId,
                user.Id,
                request.Role,
                cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await AllowedTenantCacheHelper.RemoveUserEntries(
                allowedTenantStore,
                [existingMembershipForUser.UserId],
                cancellationToken);

            return selectedMembershipId;
        }

        var tenantUserId = await EnsureWorkspaceMembershipsAsync(
            workspaceId,
            tenantId,
            user.Id,
            request.Role,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        await AllowedTenantCacheHelper.RemoveUserEntries(allowedTenantStore, [user.Id], cancellationToken);

        return tenantUserId;
    }

    private static ApiErrorException CreateExistingTenantUserEmailException()
    {
        return new ApiErrorException(
            "This email is already a member of the workspace.",
            errorCode: (int)HttpStatusCode.BadRequest);
    }

    private async Task<Guid> EnsureWorkspaceMembershipsAsync(
        Guid workspaceId,
        Guid selectedTenantId,
        Guid userId,
        TenantUserRoleType role,
        CancellationToken cancellationToken)
    {
        var tenantIds = await dbContext.Tenants
            .Where(entity => entity.WorkspaceId == workspaceId && entity.IsActive)
            .Select(entity => entity.Id)
            .ToListAsync(cancellationToken);
        var memberships = await dbContext.TenantUsers
            .Where(entity => tenantIds.Contains(entity.TenantId) && entity.UserId == userId)
            .ToListAsync(cancellationToken);

        foreach (var siblingTenantId in tenantIds)
        {
            var membership = memberships.FirstOrDefault(entity => entity.TenantId == siblingTenantId);
            if (membership is null)
            {
                membership = new TenantUser
                {
                    Id = Guid.NewGuid(),
                    TenantId = siblingTenantId,
                    UserId = userId,
                    Role = role
                };
                await dbContext.TenantUsers.AddAsync(membership, cancellationToken);
                memberships.Add(membership);
            }
            else
            {
                membership.Role = role;
            }
        }

        return memberships.Single(entity => entity.TenantId == selectedTenantId).Id;
    }

    private async Task<User> ResolveOrEnsureUserByEmailAsync(
        string normalizedEmail,
        string trimmedName,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(entity => entity.Email.ToLower() == normalizedEmail, cancellationToken);

        if (user is not null)
        {
            return user;
        }

        var userId = await dbContext.EnsureUser(
            normalizedEmail,
            trimmedName,
            normalizedEmail,
            cancellationToken);

        return await dbContext.Users.FirstAsync(entity => entity.Id == userId, cancellationToken);
    }
}
