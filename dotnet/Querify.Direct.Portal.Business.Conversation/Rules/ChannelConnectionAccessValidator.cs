using System.Net;
using Microsoft.EntityFrameworkCore;
using Querify.Common.EntityFramework.Tenant;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Models.Tenant.Enums;

namespace Querify.Direct.Portal.Business.Conversation.Rules;

public sealed class ChannelConnectionAccessValidator(TenantDbContext tenantDbContext)
{
    public async Task ValidateAsync(
        Guid tenantId,
        Guid channelConnectionId,
        CancellationToken cancellationToken)
    {
        var isAvailable = await tenantDbContext.ChannelConnections.AsNoTracking()
            .AnyAsync(
                connection =>
                    connection.Id == channelConnectionId &&
                    connection.TenantId == tenantId &&
                    connection.IsEnabled &&
                    connection.Status == ChannelConnectionStatus.Connected,
                cancellationToken);

        if (!isAvailable)
            throw new ApiErrorException(
                "The selected channel connection is not connected and available for this workspace.",
                (int)HttpStatusCode.UnprocessableEntity);
    }
}
