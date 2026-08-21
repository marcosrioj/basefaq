using MediatR;
using Querify.Direct.Portal.Business.Contact.Abstractions;
using Querify.Direct.Portal.Business.Contact.Commands.CreateContact;
using Querify.Direct.Portal.Business.Contact.Commands.DeleteContact;
using Querify.Direct.Portal.Business.Contact.Commands.UpdateContact;
using Querify.Direct.Portal.Business.Contact.Queries.GetContact;
using Querify.Direct.Portal.Business.Contact.Queries.GetContactList;
using Querify.Models.Common.Dtos;
using Querify.Models.Direct.Dtos.Contact;

namespace Querify.Direct.Portal.Business.Contact.Service;

public sealed class ContactService(IMediator mediator) : IContactService
{
    public Task<Guid> Create(ContactCreateRequestDto request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return mediator.Send(new ContactsCreateContactCommand { Request = request }, cancellationToken);
    }

    public Task<Guid> Update(Guid id, ContactUpdateRequestDto request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return mediator.Send(new ContactsUpdateContactCommand { Id = id, Request = request }, cancellationToken);
    }

    public Task Delete(Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new ContactsDeleteContactCommand { Id = id }, cancellationToken);

    public Task<ContactDto> GetById(Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new ContactsGetContactQuery { Id = id }, cancellationToken);

    public Task<PagedResultDto<ContactDto>> GetAll(
        ContactGetAllRequestDto request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return mediator.Send(new ContactsGetContactListQuery { Request = request }, cancellationToken);
    }
}
