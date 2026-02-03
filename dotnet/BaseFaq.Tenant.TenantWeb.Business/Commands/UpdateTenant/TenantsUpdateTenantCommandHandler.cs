using BaseFaq.Common.EntityFramework.Tenant;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Tenant.TenantWeb.Business.Commands.UpdateTenant;

public class TenantsUpdateTenantCommandHandler(TenantDbContext dbContext)
    : IRequestHandler<TenantsUpdateTenantCommand>
{
    public async Task Handle(TenantsUpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await dbContext.Tenants.FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (tenant is null)
        {
            throw new KeyNotFoundException($"Tenant '{request.Id}' was not found.");
        }

        tenant.Slug = request.Slug;
        tenant.Name = request.Name;
        tenant.Edition = request.Edition;
        tenant.ConnectionString = request.ConnectionString;
        tenant.IsActive = request.IsActive;

        dbContext.Tenants.Update(tenant);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}