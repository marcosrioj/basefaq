using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Direct.Common.Domain.BusinessRules.Conversations;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.Models.Common.Enums;
using ConversationMessageEntity = Querify.Direct.Common.Domain.Entities.ConversationMessage;

namespace Querify.Direct.Portal.Business.ConversationMessage.Commands.CreateConversationMessage;

public sealed class ConversationMessagesCreateCommandHandler(
    DirectDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ConversationMessagesCreateCommand, Guid>
{
    public async Task<Guid> Handle(
        ConversationMessagesCreateCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Request;
        if (!Enum.IsDefined(dto.ActorKind))
            throw new ApiErrorException("Unsupported message actor kind.", (int)HttpStatusCode.UnprocessableEntity);
        if (dto.SentAtUtc == default)
            throw new ApiErrorException("Sent time is required.", (int)HttpStatusCode.BadRequest);

        var body = dto.Body.Trim();
        if (body.Length == 0 || body.Length > ConversationMessageEntity.MaxBodyLength)
            throw new ApiErrorException(
                $"Message body must contain between 1 and {ConversationMessageEntity.MaxBodyLength} characters.",
                (int)HttpStatusCode.BadRequest);

        var tenantId = sessionService.GetTenantId(ModuleEnum.Direct);
        var conversation = await dbContext.Conversations.SingleOrDefaultAsync(
            entity => entity.Id == dto.ConversationId && entity.TenantId == tenantId,
            cancellationToken);
        if (conversation is null)
            throw new ApiErrorException(
                $"Conversation '{dto.ConversationId}' was not found.",
                (int)HttpStatusCode.UnprocessableEntity);

        ConversationRules.EnsureAcceptsMessages(conversation);
        var userId = sessionService.GetUserId().ToString("D");
        var entity = new ConversationMessageEntity
        {
            TenantId = tenantId,
            ConversationId = conversation.Id,
            Conversation = conversation,
            ActorKind = dto.ActorKind,
            Body = body,
            SentAtUtc = dto.SentAtUtc,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        dbContext.ConversationMessages.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
