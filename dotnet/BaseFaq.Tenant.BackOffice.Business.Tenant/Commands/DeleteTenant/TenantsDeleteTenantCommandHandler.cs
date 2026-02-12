using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Tenant.BackOffice.Business.Tenant.Commands.DeleteTenant;

public class TenantsDeleteTenantCommandHandler(TenantDbContext dbContext)
    : IRequestHandler<TenantsDeleteTenantCommand>
{
    public async Task Handle(TenantsDeleteTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await dbContext.Tenants
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (tenant is null)
        {
            throw new ApiErrorException(
                $"Tenant '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        dbContext.Tenants.Remove(tenant);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}