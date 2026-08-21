using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Broadcast.Common.Domain.BusinessRules.Threads;
using Querify.Broadcast.Common.Persistence.BroadcastDb.DbContext;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Models.Common.Enums;
using ItemEntity = Querify.Broadcast.Common.Domain.Entities.Item;

namespace Querify.Broadcast.Portal.Business.Item.Commands.CreateItem;

public sealed class ItemsCreateCommandHandler(
    BroadcastDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ItemsCreateCommand, Guid>
{
    public async Task<Guid> Handle(ItemsCreateCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Request;
        if (!Enum.IsDefined(dto.Kind))
            throw new ApiErrorException("Unsupported Broadcast item kind.", (int)HttpStatusCode.UnprocessableEntity);
        if (!Enum.IsDefined(dto.ActorKind))
            throw new ApiErrorException("Unsupported Broadcast actor kind.", (int)HttpStatusCode.UnprocessableEntity);
        if (dto.CapturedAtUtc == default)
            throw new ApiErrorException("Capture time is required.", (int)HttpStatusCode.BadRequest);

        var body = dto.Body.Trim();
        if (body.Length == 0 || body.Length > ItemEntity.MaxBodyLength)
            throw new ApiErrorException(
                $"Item body must contain between 1 and {ItemEntity.MaxBodyLength} characters.",
                (int)HttpStatusCode.BadRequest);

        var tenantId = sessionService.GetTenantId(ModuleEnum.Broadcast);
        var thread = await dbContext.Threads.SingleOrDefaultAsync(
            entity => entity.Id == dto.ThreadId && entity.TenantId == tenantId,
            cancellationToken);
        if (thread is null)
            throw new ApiErrorException(
                $"Thread '{dto.ThreadId}' was not found.",
                (int)HttpStatusCode.UnprocessableEntity);

        ThreadRules.EnsureAcceptsItems(thread);
        var userId = sessionService.GetUserId().ToString("D");
        var entity = new ItemEntity
        {
            TenantId = tenantId,
            ThreadId = thread.Id,
            Thread = thread,
            Kind = dto.Kind,
            ActorKind = dto.ActorKind,
            Body = body,
            CapturedAtUtc = dto.CapturedAtUtc,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        dbContext.Items.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
