using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqTag;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Abstractions;

public interface IFaqTagService
{
    Task<FaqTagDto> Create(FaqTagCreateRequestDto dto, CancellationToken token);
    Task<PagedResultDto<FaqTagDto>> GetAll(FaqTagGetAllRequestDto requestDto, CancellationToken token);
    Task<FaqTagDto> GetById(Guid id, CancellationToken token);
    Task<FaqTagDto> Update(Guid id, FaqTagUpdateRequestDto dto, CancellationToken token);
}