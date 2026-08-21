using MediatR;
using Querify.Broadcast.Common.Domain.BusinessRules.Threads;
using Querify.Broadcast.Common.Persistence.BroadcastDb.DbContext;
using Querify.Broadcast.Portal.Business.Thread.Rules;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Models.Common.Enums;
using ThreadEntity = Querify.Broadcast.Common.Domain.Entities.Thread;

namespace Querify.Broadcast.Portal.Business.Thread.Commands.CreateThread;

public sealed class ThreadsCreateCommandHandler(
    BroadcastDbContext dbContext,
    ISessionService sessionService,
    ChannelConnectionAccessValidator connectionValidator)
    : IRequestHandler<ThreadsCreateCommand, Guid>
{
    public async Task<Guid> Handle(ThreadsCreateCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Request;
        ThreadRules.EnsureSupportedStatus(dto.Status);
        var tenantId = sessionService.GetTenantId(ModuleEnum.Broadcast);
        await connectionValidator.ValidateAsync(tenantId, dto.ChannelConnectionId, cancellationToken);
        var userId = sessionService.GetUserId().ToString("D");
        var entity = new ThreadEntity
        {
            TenantId = tenantId,
            ChannelConnectionId = dto.ChannelConnectionId,
            Title = dto.Title,
            Status = dto.Status,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        dbContext.Threads.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
