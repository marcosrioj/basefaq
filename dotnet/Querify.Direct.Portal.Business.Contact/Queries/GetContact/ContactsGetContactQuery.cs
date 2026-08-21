using MediatR;
using Querify.Models.Direct.Dtos.Contact;

namespace Querify.Direct.Portal.Business.Contact.Queries.GetContact;

public sealed class ContactsGetContactQuery : IRequest<ContactDto>
{
    public required Guid Id { get; set; }
}
