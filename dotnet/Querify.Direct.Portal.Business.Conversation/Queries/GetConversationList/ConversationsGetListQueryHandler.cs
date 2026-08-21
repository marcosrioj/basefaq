using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.Models.Common.Dtos;
using Querify.Models.Common.Enums;
using Querify.Models.Direct.Dtos.Conversation;
using ConversationEntity = Querify.Direct.Common.Domain.Entities.Conversation;

namespace Querify.Direct.Portal.Business.Conversation.Queries.GetConversationList;

public sealed class ConversationsGetListQueryHandler(
    DirectDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ConversationsGetListQuery, PagedResultDto<ConversationDto>>
{
    public async Task<PagedResultDto<ConversationDto>> Handle(
        ConversationsGetListQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(ModuleEnum.Direct);
        var dto = request.Request;
        var query = dbContext.Conversations.AsNoTracking()
            .Where(conversation => conversation.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(dto.SearchText))
        {
            var search = $"%{dto.SearchText}%";
            query = query.Where(conversation =>
                EF.Functions.ILike(conversation.Subject ?? string.Empty, search) ||
                EF.Functions.ILike(conversation.Contact.GivenName, search) ||
                EF.Functions.ILike(conversation.Contact.Surname ?? string.Empty, search));
        }

        if (dto.ContactId.HasValue)
            query = query.Where(conversation => conversation.ContactId == dto.ContactId.Value);
        if (dto.ChannelConnectionId.HasValue)
            query = query.Where(conversation => conversation.ChannelConnectionId == dto.ChannelConnectionId.Value);
        if (dto.Status.HasValue)
            query = query.Where(conversation => conversation.Status == dto.Status.Value);

        query = ApplySorting(query, dto.Sorting);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(dto.SkipCount).Take(dto.MaxResultCount)
            .Select(conversation => new ConversationDto
            {
                Id = conversation.Id,
                TenantId = conversation.TenantId,
                ContactId = conversation.ContactId,
                ChannelConnectionId = conversation.ChannelConnectionId,
                Subject = conversation.Subject,
                Status = conversation.Status,
                MessageCount = conversation.Messages.Count,
                LastMessageAtUtc = conversation.Messages
                    .OrderByDescending(message => message.SentAtUtc)
                    .Select(message => (DateTime?)message.SentAtUtc)
                    .FirstOrDefault(),
                CreatedAtUtc = conversation.CreatedDate,
                LastUpdatedAtUtc = conversation.UpdatedDate ?? conversation.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<ConversationDto>(totalCount, items);
    }

    private static IQueryable<ConversationEntity> ApplySorting(
        IQueryable<ConversationEntity> query,
        string? sorting) => sorting?.Trim().ToLowerInvariant() switch
        {
            "subject" or "subject asc" => query.OrderBy(conversation => conversation.Subject),
            "subject desc" => query.OrderByDescending(conversation => conversation.Subject),
            "status" or "status asc" => query.OrderBy(conversation => conversation.Status),
            "status desc" => query.OrderByDescending(conversation => conversation.Status),
            "messagecount" or "messagecount asc" => query.OrderBy(conversation => conversation.Messages.Count),
            "messagecount desc" => query.OrderByDescending(conversation => conversation.Messages.Count),
            "lastmessageatutc asc" => query.OrderBy(conversation => conversation.Messages.Max(message => (DateTime?)message.SentAtUtc)),
            "lastmessageatutc desc" => query.OrderByDescending(conversation => conversation.Messages.Max(message => (DateTime?)message.SentAtUtc)),
            "lastupdatedatutc asc" => query.OrderBy(conversation => conversation.UpdatedDate ?? conversation.CreatedDate),
            _ => query.OrderByDescending(conversation => conversation.UpdatedDate ?? conversation.CreatedDate)
        };
}
