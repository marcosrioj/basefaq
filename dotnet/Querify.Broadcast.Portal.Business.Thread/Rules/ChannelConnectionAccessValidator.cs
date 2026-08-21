using System.Net;
using Microsoft.EntityFrameworkCore;
using Querify.Common.EntityFramework.Tenant;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Models.Common.Enums;
using Querify.Models.Tenant.Enums;

namespace Querify.Broadcast.Portal.Business.Thread.Rules;

public sealed class ChannelConnectionAccessValidator(TenantDbContext tenantDbContext)
{
    public async Task ValidateAsync(
        Guid broadcastTenantId,
        Guid channelConnectionId,
        CancellationToken cancellationToken)
    {
        var workspaceId = await tenantDbContext.Tenants.AsNoTracking()
            .Where(tenant =>
                tenant.Id == broadcastTenantId &&
                tenant.Module == ModuleEnum.Broadcast &&
                tenant.IsActive)
            .Select(tenant => (Guid?)tenant.WorkspaceId)
            .SingleOrDefaultAsync(cancellationToken);

        if (!workspaceId.HasValue)
            throw new ApiErrorException(
                "The selected Broadcast module is not available.",
                (int)HttpStatusCode.UnprocessableEntity);

        var baseTenantId = await tenantDbContext.Tenants.AsNoTracking()
            .Where(tenant =>
                tenant.WorkspaceId == workspaceId.Value &&
                tenant.Module == ModuleEnum.QnA &&
                tenant.IsActive)
            .Select(tenant => (Guid?)tenant.Id)
            .SingleOrDefaultAsync(cancellationToken);

        var isAvailable = baseTenantId.HasValue && await tenantDbContext.ChannelConnections.AsNoTracking()
            .AnyAsync(
                connection =>
                    connection.Id == channelConnectionId &&
                    connection.TenantId == baseTenantId.Value &&
                    connection.IsEnabled &&
                    connection.Status == ChannelConnectionStatus.Connected,
                cancellationToken);

        if (!isAvailable)
            throw new ApiErrorException(
                "The selected channel connection is not connected and available for this workspace.",
                (int)HttpStatusCode.UnprocessableEntity);
    }
}
