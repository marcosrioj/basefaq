using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.Models.Common.Enums;
using Querify.Models.Direct.Dtos.Contact;
using Querify.Models.Direct.Dtos.Conversation;

namespace Querify.Direct.Portal.Business.Conversation.Queries.GetConversation;

public sealed class ConversationsGetQueryHandler(
    DirectDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ConversationsGetQuery, ConversationDetailDto>
{
    public async Task<ConversationDetailDto> Handle(
        ConversationsGetQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(ModuleEnum.Direct);
        var dto = await dbContext.Conversations.AsNoTracking()
            .Where(conversation => conversation.Id == request.Id && conversation.TenantId == tenantId)
            .Select(conversation => new ConversationDetailDto
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
                LastUpdatedAtUtc = conversation.UpdatedDate ?? conversation.CreatedDate,
                Contact = new ContactDto
                {
                    Id = conversation.Contact.Id,
                    TenantId = conversation.Contact.TenantId,
                    GivenName = conversation.Contact.GivenName,
                    Surname = conversation.Contact.Surname,
                    Email = conversation.Contact.Email,
                    PhotoUrl = conversation.Contact.PhotoUrl,
                    TimeZone = conversation.Contact.TimeZone,
                    PhoneNumber = conversation.Contact.PhoneNumber,
                    InstagramProfileUrl = conversation.Contact.InstagramProfileUrl,
                    TikTokProfileUrl = conversation.Contact.TikTokProfileUrl,
                    FacebookProfileUrl = conversation.Contact.FacebookProfileUrl,
                    SnapchatProfileUrl = conversation.Contact.SnapchatProfileUrl,
                    ConversationCount = conversation.Contact.Conversations.Count,
                    CreatedAtUtc = conversation.Contact.CreatedDate,
                    LastUpdatedAtUtc = conversation.Contact.UpdatedDate ?? conversation.Contact.CreatedDate
                }
            })
            .SingleOrDefaultAsync(cancellationToken);

        return dto ?? throw new ApiErrorException(
            $"Conversation '{request.Id}' was not found.",
            (int)HttpStatusCode.NotFound);
    }
}
