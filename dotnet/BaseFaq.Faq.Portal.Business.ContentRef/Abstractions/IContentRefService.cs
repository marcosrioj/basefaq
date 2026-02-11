using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.ContentRef;

namespace BaseFaq.Faq.Portal.Business.ContentRef.Abstractions;

public interface IContentRefService
{
    Task<ContentRefDto> Create(ContentRefCreateRequestDto dto, CancellationToken token);
    Task Delete(Guid id, CancellationToken token);
    Task<PagedResultDto<ContentRefDto>> GetAll(ContentRefGetAllRequestDto requestDto, CancellationToken token);
    Task<ContentRefDto> GetById(Guid id, CancellationToken token);
    Task<ContentRefDto> Update(Guid id, ContentRefUpdateRequestDto dto, CancellationToken token);
}