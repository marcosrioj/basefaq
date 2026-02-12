using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Tenant.Portal.Business.Tenant.Queries.GetClientKey;

public class TenantsGetClientKeyQueryHandler(TenantDbContext dbContext, ISessionService sessionService)
    : IRequestHandler<TenantsGetClientKeyQuery, string?>
{
    public async Task<string?> Handle(TenantsGetClientKeyQuery request, CancellationToken cancellationToken)
    {
        var userId = sessionService.GetUserId();

        var tenant = await dbContext.Tenants
            .AsNoTracking()
            .Where(entity => entity.UserId == userId && entity.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (tenant is null)
        {
            throw new ApiErrorException(
                "Active tenant was not found for current user.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        return tenant.ClientKey;
    }
}