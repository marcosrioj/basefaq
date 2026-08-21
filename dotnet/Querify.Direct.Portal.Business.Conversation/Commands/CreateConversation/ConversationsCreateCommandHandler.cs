using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Direct.Common.Domain.BusinessRules.Conversations;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.Direct.Portal.Business.Conversation.Rules;
using Querify.Models.Common.Enums;
using ConversationEntity = Querify.Direct.Common.Domain.Entities.Conversation;

namespace Querify.Direct.Portal.Business.Conversation.Commands.CreateConversation;

public sealed class ConversationsCreateCommandHandler(
    DirectDbContext dbContext,
    ISessionService sessionService,
    ChannelConnectionAccessValidator connectionValidator)
    : IRequestHandler<ConversationsCreateCommand, Guid>
{
    public async Task<Guid> Handle(ConversationsCreateCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Request;
        ConversationRules.EnsureSupportedStatus(dto.Status);
        var tenantId = sessionService.GetTenantId(ModuleEnum.Direct);
        var contact = await dbContext.Contacts.SingleOrDefaultAsync(
            entity => entity.Id == dto.ContactId && entity.TenantId == tenantId,
            cancellationToken);
        if (contact is null)
            throw new ApiErrorException(
                $"Contact '{dto.ContactId}' was not found.",
                (int)HttpStatusCode.UnprocessableEntity);

        await connectionValidator.ValidateAsync(tenantId, dto.ChannelConnectionId, cancellationToken);
        var userId = sessionService.GetUserId().ToString("D");
        var entity = new ConversationEntity
        {
            TenantId = tenantId,
            ContactId = contact.Id,
            Contact = contact,
            ChannelConnectionId = dto.ChannelConnectionId,
            Subject = dto.Subject,
            Status = dto.Status,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        dbContext.Conversations.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
