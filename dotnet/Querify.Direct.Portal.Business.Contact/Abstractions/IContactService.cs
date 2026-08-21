using Querify.Models.Common.Dtos;
using Querify.Models.Direct.Dtos.Contact;

namespace Querify.Direct.Portal.Business.Contact.Abstractions;

public interface IContactService
{
    Task<Guid> Create(ContactCreateRequestDto request, CancellationToken cancellationToken);
    Task<Guid> Update(Guid id, ContactUpdateRequestDto request, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
    Task<ContactDto> GetById(Guid id, CancellationToken cancellationToken);
    Task<PagedResultDto<ContactDto>> GetAll(ContactGetAllRequestDto request, CancellationToken cancellationToken);
}
