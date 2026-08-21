using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Broadcast.Common.Persistence.BroadcastDb.DbContext;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Models.Common.Enums;

namespace Querify.Broadcast.Portal.Business.Thread.Commands.DeleteThread;

public sealed class ThreadsDeleteCommandHandler(
    BroadcastDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ThreadsDeleteCommand>
{
    public async Task Handle(ThreadsDeleteCommand request, CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(ModuleEnum.Broadcast);
        var entity = await dbContext.Threads.SingleOrDefaultAsync(
            thread => thread.Id == request.Id && thread.TenantId == tenantId,
            cancellationToken);
        if (entity is null)
            throw new ApiErrorException($"Thread '{request.Id}' was not found.", (int)HttpStatusCode.NotFound);

        if (await dbContext.Items.AnyAsync(
                item => item.ThreadId == entity.Id && item.TenantId == tenantId,
                cancellationToken))
            throw new ApiErrorException(
                "A thread with captured items cannot be deleted.",
                (int)HttpStatusCode.Conflict);

        dbContext.Threads.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
