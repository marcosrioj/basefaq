using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.ContentRef;

namespace BaseFaq.Faq.Portal.Business.ContentRef.Abstractions;

public interface IContentRefService
{
    Task<Guid> Create(ContentRefCreateRequestDto dto, CancellationToken token);
    Task Delete(Guid id, CancellationToken token);
    Task<PagedResultDto<ContentRefDto>> GetAll(ContentRefGetAllRequestDto requestDto, CancellationToken token);
    Task<ContentRefDto> GetById(Guid id, CancellationToken token);
    Task<Guid> Update(Guid id, ContentRefUpdateRequestDto dto, CancellationToken token);
}