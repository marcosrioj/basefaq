using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.EntityFramework.Tenant;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Models.Tenant.Dtos.ChannelConnection;
using Querify.Models.Tenant.Enums;
using Querify.Tenant.Portal.Business.ChannelConnection.Rules;
using ChannelConnectionEntity = Querify.Common.EntityFramework.Tenant.Entities.ChannelConnection;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Commands.CreateChannelConnection;

public sealed class ChannelConnectionsCreateCommandHandler(
    TenantDbContext dbContext,
    ChannelConnectionTenantResolver tenantResolver,
    ISessionService sessionService)
    : IRequestHandler<ChannelConnectionsCreateCommand, Guid>
{
    public async Task<Guid> Handle(ChannelConnectionsCreateCommand request, CancellationToken cancellationToken)
    {
        var baseTenantId = await tenantResolver.ResolveAsync(request.TenantId, true, cancellationToken);
        var dto = request.Request;
        ChannelConnectionRules.EnsureSupportedKind(dto.Kind);
        var providerKey = dto.ProviderKey.Trim();

        if (await dbContext.ChannelConnections.AnyAsync(
                connection => connection.TenantId == baseTenantId && connection.ProviderKey == providerKey,
                cancellationToken))
            throw new ApiErrorException(
                "A channel connection with this provider key already exists.",
                (int)HttpStatusCode.Conflict);

        var userId = sessionService.GetUserId().ToString("D");
        var entity = new ChannelConnectionEntity
        {
            TenantId = baseTenantId,
            Name = dto.Name.Trim(),
            ProviderKey = providerKey,
            Kind = dto.Kind,
            ConnectionData = ChannelConnectionRules.NormalizeConnectionData(dto.ConnectionData),
            Status = ChannelConnectionStatus.Pending,
            IsEnabled = dto.IsEnabled,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        dbContext.ChannelConnections.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
