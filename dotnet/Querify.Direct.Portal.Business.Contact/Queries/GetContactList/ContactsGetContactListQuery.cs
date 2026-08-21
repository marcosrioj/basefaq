using MediatR;
using Querify.Models.Common.Dtos;
using Querify.Models.Direct.Dtos.Contact;

namespace Querify.Direct.Portal.Business.Contact.Queries.GetContactList;

public sealed class ContactsGetContactListQuery : IRequest<PagedResultDto<ContactDto>>
{
    public required ContactGetAllRequestDto Request { get; set; }
}
