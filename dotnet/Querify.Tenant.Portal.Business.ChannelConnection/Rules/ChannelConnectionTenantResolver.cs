using System.Net;
using Microsoft.EntityFrameworkCore;
using Querify.Common.EntityFramework.Tenant;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Models.Common.Enums;
using Querify.Tenant.Portal.Business.Tenant.Abstractions;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Rules;

public sealed class ChannelConnectionTenantResolver(
    TenantDbContext dbContext,
    ITenantPortalAccessService tenantPortalAccessService)
{
    public async Task<Guid> ResolveAsync(
        Guid selectedTenantId,
        bool requiresOwner,
        CancellationToken cancellationToken)
    {
        var selectedTenant = await tenantPortalAccessService.GetAccessibleTenantAsync(
            selectedTenantId,
            cancellationToken);
        var baseTenantId = await dbContext.Tenants
            .AsNoTracking()
            .Where(tenant =>
                tenant.WorkspaceId == selectedTenant.WorkspaceId &&
                tenant.Module == ModuleEnum.QnA &&
                tenant.IsActive)
            .Select(tenant => (Guid?)tenant.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (!baseTenantId.HasValue)
            throw new ApiErrorException(
                "The workspace does not have an active QnA base tenant.",
                (int)HttpStatusCode.UnprocessableEntity);

        if (requiresOwner)
            await tenantPortalAccessService.EnsureOwnerAccessAsync(baseTenantId.Value, cancellationToken);
        else
            await tenantPortalAccessService.EnsureAccessAsync(baseTenantId.Value, cancellationToken);

        return baseTenantId.Value;
    }
}
