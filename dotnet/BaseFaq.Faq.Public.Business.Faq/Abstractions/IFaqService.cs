using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.Faq;

namespace BaseFaq.Faq.Public.Business.Faq.Abstractions;

public interface IFaqService
{
    Task<PagedResultDto<FaqDetailDto>> GetAll(FaqGetAllRequestDto requestDto, CancellationToken token);
    Task<FaqDetailDto> GetById(Guid id, FaqGetRequestDto requestDto, CancellationToken token);
}