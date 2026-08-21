using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Direct.Common.Domain.BusinessRules.Conversations;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.Direct.Portal.Business.Conversation.Rules;
using Querify.Models.Common.Enums;

namespace Querify.Direct.Portal.Business.Conversation.Commands.UpdateConversation;

public sealed class ConversationsUpdateCommandHandler(
    DirectDbContext dbContext,
    ISessionService sessionService,
    ChannelConnectionAccessValidator connectionValidator)
    : IRequestHandler<ConversationsUpdateCommand, Guid>
{
    public async Task<Guid> Handle(ConversationsUpdateCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Request;
        ConversationRules.EnsureSupportedStatus(dto.Status);
        var tenantId = sessionService.GetTenantId(ModuleEnum.Direct);
        var entity = await dbContext.Conversations.SingleOrDefaultAsync(
            conversation => conversation.Id == request.Id && conversation.TenantId == tenantId,
            cancellationToken);
        if (entity is null)
            throw new ApiErrorException(
                $"Conversation '{request.Id}' was not found.",
                (int)HttpStatusCode.NotFound);

        if (!await dbContext.Contacts.AnyAsync(
                contact => contact.Id == dto.ContactId && contact.TenantId == tenantId,
                cancellationToken))
            throw new ApiErrorException(
                $"Contact '{dto.ContactId}' was not found.",
                (int)HttpStatusCode.UnprocessableEntity);

        await connectionValidator.ValidateAsync(tenantId, dto.ChannelConnectionId, cancellationToken);
        entity.ContactId = dto.ContactId;
        entity.ChannelConnectionId = dto.ChannelConnectionId;
        entity.Subject = dto.Subject;
        entity.Status = dto.Status;
        entity.UpdatedBy = sessionService.GetUserId().ToString("D");
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
