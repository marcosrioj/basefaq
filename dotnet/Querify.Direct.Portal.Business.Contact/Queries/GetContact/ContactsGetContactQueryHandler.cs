using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.Models.Common.Enums;
using Querify.Models.Direct.Dtos.Contact;

namespace Querify.Direct.Portal.Business.Contact.Queries.GetContact;

public sealed class ContactsGetContactQueryHandler(
    DirectDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ContactsGetContactQuery, ContactDto>
{
    public async Task<ContactDto> Handle(ContactsGetContactQuery request, CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(ModuleEnum.Direct);
        var entity = await dbContext.Contacts
            .AsNoTracking()
            .Where(contact => contact.TenantId == tenantId && contact.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        return entity ?? throw new ApiErrorException(
            $"Contact '{request.Id}' was not found.",
            (int)HttpStatusCode.NotFound);
    }
}
