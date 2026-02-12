using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Common.EntityFramework.Tenant.Helpers;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Common.Enums;
using BaseFaq.Models.Tenant.Dtos.Tenant;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Tenant.Portal.Business.Tenant.Commands.CreateOrUpdateTenants;

public class TenantsCreateOrUpdateTenantsCommandHandler(TenantDbContext dbContext, ISessionService sessionService)
    : IRequestHandler<TenantsCreateOrUpdateTenantsCommand, List<TenantSummaryDto>>
{
    public async Task<List<TenantSummaryDto>> Handle(TenantsCreateOrUpdateTenantsCommand request,
        CancellationToken cancellationToken)
    {
        var userId = sessionService.GetUserId();

        var currentConnections = await dbContext.TenantConnections
            .AsNoTracking()
            .Where(entity => entity.IsCurrent)
            .ToDictionaryAsync(entity => entity.App, entity => entity.ConnectionString, cancellationToken);

        var apps = Enum.GetValues<AppEnum>().Where(app => app != AppEnum.Tenant);

        foreach (var app in apps)
        {
            if (!currentConnections.TryGetValue(app, out var connectionString) ||
                string.IsNullOrWhiteSpace(connectionString))
            {
                continue;
            }

            var activeTenant = await dbContext.Tenants
                .FirstOrDefaultAsync(
                    entity => entity.UserId == userId && entity.App == app && entity.IsActive,
                    cancellationToken);

            var slug = TenantHelper.GenerateSlug($"{request.Name}{app}");

            if (activeTenant is null)
            {
                activeTenant = new Common.EntityFramework.Tenant.Entities.Tenant
                {
                    Slug = slug,
                    Name = request.Name,
                    Edition = request.Edition,
                    App = app,
                    ConnectionString = connectionString,
                    IsActive = true,
                    UserId = userId
                };

                await dbContext.Tenants.AddAsync(activeTenant, cancellationToken);
            }
            else
            {
                activeTenant.Slug = slug;
                activeTenant.Name = request.Name;
                activeTenant.Edition = request.Edition;
                activeTenant.ConnectionString = connectionString;
                activeTenant.IsActive = true;

                dbContext.Tenants.Update(activeTenant);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return await dbContext.Tenants
            .AsNoTracking()
            .Where(entity => entity.UserId == userId && entity.IsActive)
            .OrderBy(entity => entity.App)
            .Select(tenant => new TenantSummaryDto
            {
                Id = tenant.Id,
                Slug = tenant.Slug,
                Name = tenant.Name,
                Edition = tenant.Edition,
                App = tenant.App,
                IsActive = tenant.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}