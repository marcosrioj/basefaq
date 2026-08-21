using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Direct.Common.Persistence.DirectDb.DbContext;
using Querify.Models.Common.Enums;

namespace Querify.Direct.Portal.Business.Contact.Commands.DeleteContact;

public sealed class ContactsDeleteContactCommandHandler(
    DirectDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ContactsDeleteContactCommand>
{
    public async Task Handle(ContactsDeleteContactCommand request, CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(ModuleEnum.Direct);
        var entity = await dbContext.Contacts.FirstOrDefaultAsync(
            contact => contact.TenantId == tenantId && contact.Id == request.Id,
            cancellationToken);

        if (entity is null)
            throw new ApiErrorException($"Contact '{request.Id}' was not found.", (int)HttpStatusCode.NotFound);

        if (await dbContext.Conversations.AnyAsync(
                conversation => conversation.TenantId == tenantId && conversation.ContactId == entity.Id,
                cancellationToken))
            throw new ApiErrorException(
                "A contact with conversations cannot be deleted.",
                (int)HttpStatusCode.Conflict);

        dbContext.Contacts.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
