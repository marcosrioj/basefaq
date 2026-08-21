using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.Models.Common.Enums;

namespace Querify.Direct.Portal.Business.Conversation.Commands.DeleteConversation;

public sealed class ConversationsDeleteCommandHandler(
    DirectDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ConversationsDeleteCommand>
{
    public async Task Handle(ConversationsDeleteCommand request, CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(ModuleEnum.Direct);
        var entity = await dbContext.Conversations.SingleOrDefaultAsync(
            conversation => conversation.Id == request.Id && conversation.TenantId == tenantId,
            cancellationToken);
        if (entity is null)
            throw new ApiErrorException(
                $"Conversation '{request.Id}' was not found.",
                (int)HttpStatusCode.NotFound);

        if (await dbContext.ConversationMessages.AnyAsync(
                message => message.ConversationId == entity.Id && message.TenantId == tenantId,
                cancellationToken))
            throw new ApiErrorException(
                "A conversation with messages cannot be deleted.",
                (int)HttpStatusCode.Conflict);

        dbContext.Conversations.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
