using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.EntityFramework.Tenant;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Tenant.Portal.Business.ChannelConnection.Rules;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Commands.UpdateChannelConnection;

public sealed class ChannelConnectionsUpdateCommandHandler(
    TenantDbContext dbContext,
    ChannelConnectionTenantResolver tenantResolver,
    ISessionService sessionService)
    : IRequestHandler<ChannelConnectionsUpdateCommand, Guid>
{
    public async Task<Guid> Handle(ChannelConnectionsUpdateCommand request, CancellationToken cancellationToken)
    {
        var baseTenantId = await tenantResolver.ResolveAsync(request.TenantId, true, cancellationToken);
        var entity = await dbContext.ChannelConnections.SingleOrDefaultAsync(
            connection => connection.Id == request.Id && connection.TenantId == baseTenantId,
            cancellationToken);

        if (entity is null)
            throw new ApiErrorException(
                $"Channel connection '{request.Id}' was not found.",
                (int)HttpStatusCode.NotFound);

        var dto = request.Request;
        ChannelConnectionRules.EnsureSupportedKind(dto.Kind);
        var providerKey = dto.ProviderKey.Trim();
        if (await dbContext.ChannelConnections.AnyAsync(
                connection =>
                    connection.Id != entity.Id &&
                    connection.TenantId == baseTenantId &&
                    connection.ProviderKey == providerKey,
                cancellationToken))
            throw new ApiErrorException(
                "A channel connection with this provider key already exists.",
                (int)HttpStatusCode.Conflict);

        var providerIdentityChanged = entity.Kind != dto.Kind || entity.ProviderKey != providerKey;
        entity.Name = dto.Name.Trim();
        entity.ProviderKey = providerKey;
        entity.Kind = dto.Kind;
        entity.IsEnabled = dto.IsEnabled;

        if (dto.ConnectionData is not null)
        {
            entity.ConnectionData = ChannelConnectionRules.NormalizeConnectionData(dto.ConnectionData);
            providerIdentityChanged = true;
        }

        if (providerIdentityChanged)
            ChannelConnectionRules.ResetOperationalState(entity);

        entity.UpdatedBy = sessionService.GetUserId().ToString("D");
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
