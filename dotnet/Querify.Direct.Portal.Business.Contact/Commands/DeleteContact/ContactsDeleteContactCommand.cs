using MediatR;

namespace Querify.Direct.Portal.Business.Contact.Commands.DeleteContact;

public sealed class ContactsDeleteContactCommand : IRequest
{
    public required Guid Id { get; set; }
}
