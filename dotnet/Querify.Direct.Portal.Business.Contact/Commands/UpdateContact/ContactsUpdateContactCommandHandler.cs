using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.Models.Common.Enums;
using Querify.Models.Direct.Dtos.Contact;
using ContactEntity = Querify.Direct.Common.Domain.Entities.Contact;

namespace Querify.Direct.Portal.Business.Contact.Commands.UpdateContact;

public sealed class ContactsUpdateContactCommandHandler(
    DirectDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ContactsUpdateContactCommand, Guid>
{
    public async Task<Guid> Handle(ContactsUpdateContactCommand request, CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(ModuleEnum.Direct);
        var entity = await dbContext.Contacts.FirstOrDefaultAsync(
            contact => contact.TenantId == tenantId && contact.Id == request.Id,
            cancellationToken);

        if (entity is null)
            throw new ApiErrorException($"Contact '{request.Id}' was not found.", (int)HttpStatusCode.NotFound);

        Apply(entity, request.Request);
        entity.UpdatedBy = sessionService.GetUserId().ToString("D");
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    private static void Apply(ContactEntity entity, ContactUpdateRequestDto request)
    {
        entity.GivenName = request.GivenName;
        entity.Surname = request.Surname;
        entity.Email = request.Email;
        entity.PhotoUrl = request.PhotoUrl;
        entity.TimeZone = request.TimeZone;
        entity.PhoneNumber = request.PhoneNumber;
        entity.InstagramProfileUrl = request.InstagramProfileUrl;
        entity.TikTokProfileUrl = request.TikTokProfileUrl;
        entity.FacebookProfileUrl = request.FacebookProfileUrl;
        entity.SnapchatProfileUrl = request.SnapchatProfileUrl;
    }
}
