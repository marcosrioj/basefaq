using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Common.EntityFramework.Tenant.Entities;
using BaseFaq.Common.EntityFramework.Tenant.Helpers;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Tenant.Dtos.Tenant;
using BaseFaq.Models.Tenant.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Tenant.TenantWeb.Business.Commands.SetDefaultTenant;

public class TenantsSetDefaultTenantCommandHandler(
    TenantDbContext dbContext,
    IClaimService claimService,
    ISessionService sessionService)
    : IRequestHandler<TenantsSetDefaultTenantCommand, TenantDto>
{
    public async Task<TenantDto> Handle(TenantsSetDefaultTenantCommand request, CancellationToken cancellationToken)
    {
        if (request.TenantId.HasValue)
        {
            var tenant = await dbContext.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(entity => entity.Id == request.TenantId.Value, cancellationToken);

            if (tenant is null)
            {
                throw new KeyNotFoundException($"Tenant '{request.TenantId}' was not found.");
            }

            sessionService.Set(request.TenantId, claimService.GetExternalUserId());

            return new TenantDto
            {
                Id = tenant.Id,
                Slug = tenant.Slug,
                Name = tenant.Name,
                Edition = tenant.Edition,
                ConnectionString = string.Empty,
                IsActive = tenant.IsActive
            };
        }

        var userName = claimService.GetName();
        var tenantName = string.IsNullOrWhiteSpace(userName)
            ? Common.EntityFramework.Tenant.Entities.Tenant.DefaultTenantName
            : $"{userName} Default";

        var currentTenantConnection = await dbContext.GetCurrentTenantConnection(cancellationToken);

        var newTenant = new Common.EntityFramework.Tenant.Entities.Tenant
        {
            Name = tenantName,
            Slug = TenantHelper.GenerateSlug(tenantName),
            Edition = TenantEdition.Free,
            ConnectionString = currentTenantConnection.ConnectionString,
            IsActive = true
        };

        await dbContext.Tenants.AddAsync(newTenant, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        sessionService.Set(newTenant.Id, claimService.GetExternalUserId());

        return new TenantDto
        {
            Id = newTenant.Id,
            Slug = newTenant.Slug,
            Name = newTenant.Name,
            Edition = newTenant.Edition,
            ConnectionString = string.Empty,
            IsActive = newTenant.IsActive
        };
    }
}