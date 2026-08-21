using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.EntityFramework.Tenant;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Models.Tenant.Dtos.ChannelConnection;
using Querify.Tenant.Portal.Business.ChannelConnection.Rules;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Queries.GetChannelConnection;

public sealed class ChannelConnectionsGetQueryHandler(
    TenantDbContext dbContext,
    ChannelConnectionTenantResolver tenantResolver)
    : IRequestHandler<ChannelConnectionsGetQuery, ChannelConnectionDto>
{
    public async Task<ChannelConnectionDto> Handle(
        ChannelConnectionsGetQuery request,
        CancellationToken cancellationToken)
    {
        var baseTenantId = await tenantResolver.ResolveAsync(request.TenantId, false, cancellationToken);
        var dto = await dbContext.ChannelConnections
            .AsNoTracking()
            .Where(connection => connection.Id == request.Id && connection.TenantId == baseTenantId)
            .Select(connection => new ChannelConnectionDto
            {
                Id = connection.Id,
                TenantId = connection.TenantId,
                Name = connection.Name,
                ProviderKey = connection.ProviderKey,
                Kind = connection.Kind,
                Status = connection.Status,
                IsEnabled = connection.IsEnabled,
                CredentialsExpireAtUtc = connection.CredentialsExpireAtUtc,
                LastCredentialsRefreshAtUtc = connection.LastCredentialsRefreshAtUtc,
                LastConnectedAtUtc = connection.LastConnectedAtUtc,
                LastSynchronizedAtUtc = connection.LastSynchronizedAtUtc,
                LastErrorAtUtc = connection.LastErrorAtUtc,
                LastErrorMessage = connection.LastErrorMessage,
                CreatedAtUtc = connection.CreatedDate,
                LastUpdatedAtUtc = connection.UpdatedDate ?? connection.CreatedDate
            })
            .SingleOrDefaultAsync(cancellationToken);

        return dto ?? throw new ApiErrorException(
            $"Channel connection '{request.Id}' was not found.",
            (int)HttpStatusCode.NotFound);
    }
}
