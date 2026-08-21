using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.EntityFramework.Tenant;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using ChannelConnectionEntity = Querify.Common.EntityFramework.Tenant.Entities.ChannelConnection;

namespace Querify.Tenant.BackOffice.Business.ChannelConnection.Commands.UpdateOperationalState;

public sealed class ChannelConnectionsUpdateOperationalStateCommandHandler(
    TenantDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ChannelConnectionsUpdateOperationalStateCommand, Guid>
{
    public async Task<Guid> Handle(
        ChannelConnectionsUpdateOperationalStateCommand request,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.Request.Status))
            throw new ApiErrorException(
                "Unsupported channel connection status.",
                (int)HttpStatusCode.UnprocessableEntity);

        var entity = await dbContext.ChannelConnections.SingleOrDefaultAsync(
            connection => connection.Id == request.Id,
            cancellationToken);
        if (entity is null)
            throw new ApiErrorException(
                $"Channel connection '{request.Id}' was not found.",
                (int)HttpStatusCode.NotFound);

        var errorMessage = request.Request.LastErrorMessage?.Trim();
        if (errorMessage?.Length > ChannelConnectionEntity.MaxLastErrorMessageLength)
            throw new ApiErrorException(
                $"Last error message cannot exceed {ChannelConnectionEntity.MaxLastErrorMessageLength} characters.",
                (int)HttpStatusCode.BadRequest);

        entity.Status = request.Request.Status;
        entity.CredentialsExpireAtUtc = request.Request.CredentialsExpireAtUtc;
        entity.LastCredentialsRefreshAtUtc = request.Request.LastCredentialsRefreshAtUtc;
        entity.LastConnectedAtUtc = request.Request.LastConnectedAtUtc;
        entity.LastSynchronizedAtUtc = request.Request.LastSynchronizedAtUtc;
        entity.LastErrorAtUtc = request.Request.LastErrorAtUtc;
        entity.LastErrorMessage = errorMessage;
        entity.UpdatedBy = sessionService.GetUserId().ToString("D");

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
