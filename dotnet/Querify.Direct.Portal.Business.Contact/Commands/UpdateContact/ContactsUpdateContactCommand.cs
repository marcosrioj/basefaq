using MediatR;
using Querify.Models.Direct.Dtos.Contact;

namespace Querify.Direct.Portal.Business.Contact.Commands.UpdateContact;

public sealed class ContactsUpdateContactCommand : IRequest<Guid>
{
    public required Guid Id { get; set; }
    public required ContactUpdateRequestDto Request { get; set; }
}
