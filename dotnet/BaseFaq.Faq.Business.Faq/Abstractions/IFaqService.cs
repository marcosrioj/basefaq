using BaseFaq.Models.Faq.Dtos.Faq;
using BaseFaq.Models.Common.Dtos;

namespace BaseFaq.Faq.Business.Faq.Abstractions;

public interface IFaqService
{
    Task<FaqDto> Create(FaqCreateRequestDto dto, CancellationToken token);
    Task<PagedResultDto<FaqDto>> GetAll(FaqGetAllRequestDto requestDto, CancellationToken token);
    Task<FaqDto> GetById(Guid id, CancellationToken token);
    Task<FaqDto> Update(Guid id, FaqUpdateRequestDto dto, CancellationToken token);
}