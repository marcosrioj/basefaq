using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Common.EntityFramework.Tenant.Entities;
using BaseFaq.Common.EntityFramework.Tenant.Helpers;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Common.Enums;
using BaseFaq.Models.Tenant.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Tenant.TenantWeb.Business.Commands.SetDefaultTenant;

public class TenantsSetDefaultTenantCommandHandler(
    TenantDbContext dbContext,
    IClaimService claimService,
    ISessionService sessionService)
    : IRequestHandler<TenantsSetDefaultTenantCommand, bool>
{
    public async Task<bool> Handle(TenantsSetDefaultTenantCommand request, CancellationToken cancellationToken)
    {
        var user = await EnsureUserAsync(cancellationToken);

        var userTenants = await GetTenantByUserIdAsync(user.Id, cancellationToken);

        foreach (AppEnum app in Enum.GetValues<AppEnum>())
        {
            if (app == AppEnum.TenantWeb)
                continue;

            var tenant = userTenants.FirstOrDefault(entity => entity.App == app);

            if (tenant is null)
            {
                tenant = await EnsureDefaultTenantAsync(user, app, cancellationToken);
            }

            sessionService.Set(tenant.Id, app, user.Id);
        }

        return true;
    }

    private async Task<ICollection<Common.EntityFramework.Tenant.Entities.Tenant>> GetTenantByUserIdAsync(Guid userId,
        CancellationToken cancellationToken)
    {
        var tenants = await dbContext.Tenants
            .AsNoTracking()
            .Where(entity => entity.UserId == userId && entity.IsActive)
            .ToListAsync(cancellationToken);

        return tenants;
    }

    private async Task<User> EnsureUserAsync(CancellationToken cancellationToken)
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
            return user;
        }

        user = new User
        {
            Id = Guid.NewGuid(),
            GivenName = userName,
            ExternalId = externalUserId,
            Email = email,
            Role = UserRoleType.Member
        };

        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }

    private async Task<Common.EntityFramework.Tenant.Entities.Tenant> EnsureDefaultTenantAsync(
        User user, AppEnum app, CancellationToken cancellationToken)
    {
        var tenant = await dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.UserId == user.Id && entity.App == app && entity.IsActive,
                cancellationToken);

        if (tenant is not null)
        {
            return tenant;
        }

        var currentTenantConnection = await dbContext.GetCurrentTenantConnection(app, cancellationToken);

        var tenantName = $"{user.GivenName} {Common.EntityFramework.Tenant.Entities.Tenant.DefaultTenantName}";

        tenant = new Common.EntityFramework.Tenant.Entities.Tenant
        {
            Name = tenantName,
            Slug = TenantHelper.GenerateSlug(tenantName),
            Edition = TenantEdition.Free,
            App = app,
            ConnectionString = currentTenantConnection.ConnectionString,
            IsActive = true,
            UserId = user.Id
        };

        await dbContext.Tenants.AddAsync(tenant, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return tenant;
    }
}