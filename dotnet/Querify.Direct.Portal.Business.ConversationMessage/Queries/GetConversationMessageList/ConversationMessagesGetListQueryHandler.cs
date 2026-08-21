using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.Models.Common.Dtos;
using Querify.Models.Common.Enums;
using Querify.Models.Direct.Dtos.ConversationMessage;

namespace Querify.Direct.Portal.Business.ConversationMessage.Queries.GetConversationMessageList;

public sealed class ConversationMessagesGetListQueryHandler(
    DirectDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ConversationMessagesGetListQuery, PagedResultDto<ConversationMessageDto>>
{
    public async Task<PagedResultDto<ConversationMessageDto>> Handle(
        ConversationMessagesGetListQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(ModuleEnum.Direct);
        var dto = request.Request;
        if (!await dbContext.Conversations.AsNoTracking().AnyAsync(
                conversation => conversation.Id == dto.ConversationId && conversation.TenantId == tenantId,
                cancellationToken))
            throw new ApiErrorException(
                $"Conversation '{dto.ConversationId}' was not found.",
                (int)HttpStatusCode.NotFound);

        var query = dbContext.ConversationMessages.AsNoTracking()
            .Where(message => message.TenantId == tenantId && message.ConversationId == dto.ConversationId);
        if (dto.ActorKind.HasValue)
            query = query.Where(message => message.ActorKind == dto.ActorKind.Value);

        query = dto.Sorting?.Trim().ToLowerInvariant() switch
        {
            "sentatutc asc" => query.OrderBy(message => message.SentAtUtc),
            "createdatutc asc" => query.OrderBy(message => message.CreatedDate),
            "createdatutc desc" => query.OrderByDescending(message => message.CreatedDate),
            _ => query.OrderByDescending(message => message.SentAtUtc)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(dto.SkipCount).Take(dto.MaxResultCount)
            .Select(message => new ConversationMessageDto
            {
                Id = message.Id,
                TenantId = message.TenantId,
                ConversationId = message.ConversationId,
                ActorKind = message.ActorKind,
                Body = message.Body,
                SentAtUtc = message.SentAtUtc,
                CreatedAtUtc = message.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<ConversationMessageDto>(totalCount, items);
    }
}
