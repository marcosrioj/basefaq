using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Broadcast.Common.Domain.BusinessRules.Threads;
using Querify.Broadcast.Common.Persistence.BroadcastDb.DbContext;
using Querify.Broadcast.Portal.Business.Thread.Rules;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Models.Common.Enums;

namespace Querify.Broadcast.Portal.Business.Thread.Commands.UpdateThread;

public sealed class ThreadsUpdateCommandHandler(
    BroadcastDbContext dbContext,
    ISessionService sessionService,
    ChannelConnectionAccessValidator connectionValidator)
    : IRequestHandler<ThreadsUpdateCommand, Guid>
{
    public async Task<Guid> Handle(ThreadsUpdateCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Request;
        ThreadRules.EnsureSupportedStatus(dto.Status);
        var tenantId = sessionService.GetTenantId(ModuleEnum.Broadcast);
        var entity = await dbContext.Threads.SingleOrDefaultAsync(
            thread => thread.Id == request.Id && thread.TenantId == tenantId,
            cancellationToken);
        if (entity is null)
            throw new ApiErrorException($"Thread '{request.Id}' was not found.", (int)HttpStatusCode.NotFound);

        await connectionValidator.ValidateAsync(tenantId, dto.ChannelConnectionId, cancellationToken);
        entity.ChannelConnectionId = dto.ChannelConnectionId;
        entity.Title = dto.Title;
        entity.Status = dto.Status;
        entity.UpdatedBy = sessionService.GetUserId().ToString("D");
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
