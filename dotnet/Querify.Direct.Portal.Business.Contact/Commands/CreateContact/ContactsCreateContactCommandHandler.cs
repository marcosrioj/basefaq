using MediatR;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.Models.Common.Enums;
using Querify.Models.Direct.Dtos.Contact;
using ContactEntity = Querify.Direct.Common.Domain.Entities.Contact;

namespace Querify.Direct.Portal.Business.Contact.Commands.CreateContact;

public sealed class ContactsCreateContactCommandHandler(
    DirectDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ContactsCreateContactCommand, Guid>
{
    public async Task<Guid> Handle(ContactsCreateContactCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Request);

        var userId = sessionService.GetUserId().ToString("D");
        var entity = new ContactEntity
        {
            TenantId = sessionService.GetTenantId(ModuleEnum.Direct),
            GivenName = request.Request.GivenName,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        Apply(entity, request.Request);
        dbContext.Contacts.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    private static void Apply(ContactEntity entity, ContactCreateRequestDto request)
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
