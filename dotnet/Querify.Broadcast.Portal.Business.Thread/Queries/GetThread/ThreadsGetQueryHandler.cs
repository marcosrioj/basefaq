using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Broadcast.Common.Persistence.BroadcastDb.DbContext;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Models.Broadcast.Dtos.Thread;
using Querify.Models.Common.Enums;

namespace Querify.Broadcast.Portal.Business.Thread.Queries.GetThread;

public sealed class ThreadsGetQueryHandler(
    BroadcastDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ThreadsGetQuery, ThreadDto>
{
    public async Task<ThreadDto> Handle(ThreadsGetQuery request, CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(ModuleEnum.Broadcast);
        var dto = await dbContext.Threads.AsNoTracking()
            .Where(thread => thread.Id == request.Id && thread.TenantId == tenantId)
            .Select(thread => new ThreadDto
            {
                Id = thread.Id,
                TenantId = thread.TenantId,
                ChannelConnectionId = thread.ChannelConnectionId,
                Title = thread.Title,
                Status = thread.Status,
                ItemCount = thread.Items.Count,
                LastItemAtUtc = thread.Items
                    .OrderByDescending(item => item.CapturedAtUtc)
                    .Select(item => (DateTime?)item.CapturedAtUtc)
                    .FirstOrDefault(),
                CreatedAtUtc = thread.CreatedDate,
                LastUpdatedAtUtc = thread.UpdatedDate ?? thread.CreatedDate
            })
            .SingleOrDefaultAsync(cancellationToken);

        return dto ?? throw new ApiErrorException(
            $"Thread '{request.Id}' was not found.",
            (int)HttpStatusCode.NotFound);
    }
}
