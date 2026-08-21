using Querify.Tenant.Portal.Business.Tenant.Abstractions;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Rules;

public sealed class ChannelConnectionTenantResolver(ITenantPortalAccessService tenantPortalAccessService)
{
    public async Task<Guid> ResolveAsync(
        Guid selectedTenantId,
        bool requiresOwner,
        CancellationToken cancellationToken)
    {
        if (requiresOwner)
            await tenantPortalAccessService.EnsureOwnerAccessAsync(selectedTenantId, cancellationToken);
        else
            await tenantPortalAccessService.EnsureAccessAsync(selectedTenantId, cancellationToken);

        return selectedTenantId;
    }
}
