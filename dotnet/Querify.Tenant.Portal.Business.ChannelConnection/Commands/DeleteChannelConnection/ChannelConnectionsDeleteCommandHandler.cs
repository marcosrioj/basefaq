using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.EntityFramework.Tenant;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Tenant.Portal.Business.ChannelConnection.Rules;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Commands.DeleteChannelConnection;

public sealed class ChannelConnectionsDeleteCommandHandler(
    TenantDbContext dbContext,
    ChannelConnectionTenantResolver tenantResolver)
    : IRequestHandler<ChannelConnectionsDeleteCommand>
{
    public async Task Handle(ChannelConnectionsDeleteCommand request, CancellationToken cancellationToken)
    {
        var baseTenantId = await tenantResolver.ResolveAsync(request.TenantId, true, cancellationToken);
        var entity = await dbContext.ChannelConnections.SingleOrDefaultAsync(
            connection => connection.Id == request.Id && connection.TenantId == baseTenantId,
            cancellationToken);

        if (entity is null)
            throw new ApiErrorException(
                $"Channel connection '{request.Id}' was not found.",
                (int)HttpStatusCode.NotFound);

        dbContext.ChannelConnections.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
