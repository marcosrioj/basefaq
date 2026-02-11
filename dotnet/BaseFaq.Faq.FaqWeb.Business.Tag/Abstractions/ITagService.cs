using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.Tag;

namespace BaseFaq.Faq.FaqWeb.Business.Tag.Abstractions;

public interface ITagService
{
    Task<TagDto> Create(TagCreateRequestDto dto, CancellationToken token);
    Task Delete(Guid id, CancellationToken token);
    Task<PagedResultDto<TagDto>> GetAll(TagGetAllRequestDto requestDto, CancellationToken token);
    Task<TagDto> GetById(Guid id, CancellationToken token);
    Task<TagDto> Update(Guid id, TagUpdateRequestDto dto, CancellationToken token);
}