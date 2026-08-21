using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.Models.Common.Dtos;
using Querify.Models.Common.Enums;
using Querify.Models.Direct.Dtos.Contact;

namespace Querify.Direct.Portal.Business.Contact.Queries.GetContactList;

public sealed class ContactsGetContactListQueryHandler(
    DirectDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ContactsGetContactListQuery, PagedResultDto<ContactDto>>
{
    public async Task<PagedResultDto<ContactDto>> Handle(
        ContactsGetContactListQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(ModuleEnum.Direct);
        var query = dbContext.Contacts.AsNoTracking().Where(contact => contact.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(request.Request.SearchText))
        {
            var search = $"%{request.Request.SearchText}%";
            query = query.Where(contact =>
                EF.Functions.ILike(contact.GivenName, search) ||
                EF.Functions.ILike(contact.Surname ?? string.Empty, search) ||
                EF.Functions.ILike(contact.Email ?? string.Empty, search) ||
                EF.Functions.ILike(contact.PhoneNumber ?? string.Empty, search));
        }

        query = request.Request.Sorting?.Trim().ToLowerInvariant() switch
        {
            "name" or "name asc" => query.OrderBy(contact => contact.GivenName).ThenBy(contact => contact.Surname),
            "name desc" => query.OrderByDescending(contact => contact.GivenName)
                .ThenByDescending(contact => contact.Surname),
            "conversationcount" or "conversationcount asc" => query.OrderBy(contact => contact.Conversations.Count),
            "conversationcount desc" => query.OrderByDescending(contact => contact.Conversations.Count),
            "lastupdatedatutc asc" => query.OrderBy(contact => contact.UpdatedDate ?? contact.CreatedDate),
            _ => query.OrderByDescending(contact => contact.UpdatedDate ?? contact.CreatedDate)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(request.Request.SkipCount)
            .Take(request.Request.MaxResultCount)
            .Select(contact => new ContactDto
            {
                Id = contact.Id,
                TenantId = contact.TenantId,
                GivenName = contact.GivenName,
                Surname = contact.Surname,
                Email = contact.Email,
                PhotoUrl = contact.PhotoUrl,
                TimeZone = contact.TimeZone,
                PhoneNumber = contact.PhoneNumber,
                InstagramProfileUrl = contact.InstagramProfileUrl,
                TikTokProfileUrl = contact.TikTokProfileUrl,
                FacebookProfileUrl = contact.FacebookProfileUrl,
                SnapchatProfileUrl = contact.SnapchatProfileUrl,
                ConversationCount = contact.Conversations.Count,
                CreatedAtUtc = contact.CreatedDate,
                LastUpdatedAtUtc = contact.UpdatedDate ?? contact.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<ContactDto>(totalCount, items);
    }
}
