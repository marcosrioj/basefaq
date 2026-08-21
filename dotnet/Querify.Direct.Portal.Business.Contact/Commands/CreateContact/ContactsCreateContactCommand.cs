using MediatR;
using Querify.Models.Direct.Dtos.Contact;

namespace Querify.Direct.Portal.Business.Contact.Commands.CreateContact;

public sealed class ContactsCreateContactCommand : IRequest<Guid>
{
    public required ContactCreateRequestDto Request { get; set; }
}
