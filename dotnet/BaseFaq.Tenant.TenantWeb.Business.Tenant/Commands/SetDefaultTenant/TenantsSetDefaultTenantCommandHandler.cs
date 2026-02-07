using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Common.EntityFramework.Tenant.Helpers;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Common.Enums;
using BaseFaq.Models.Tenant.Enums;
using BaseFaq.Tenant.TenantWeb.Business.User.Commands.EnsureUser;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Tenant.TenantWeb.Business.Tenant.Commands.SetDefaultTenant;

public class TenantsSetDefaultTenantCommandHandler(
    TenantDbContext dbContext,
    ISessionService sessionService,
    IMediator mediator)
    : IRequestHandler<TenantsSetDefaultTenantCommand, bool>
{
    public async Task<bool> Handle(TenantsSetDefaultTenantCommand request, CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new UsersEnsureUserCommand(), cancellationToken);

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

    private async Task<Common.EntityFramework.Tenant.Entities.Tenant> EnsureDefaultTenantAsync(
        Common.EntityFramework.Tenant.Entities.User user, AppEnum app, CancellationToken cancellationToken)
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