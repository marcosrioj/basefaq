using BaseFaq.Models.Faq.Dtos.Faq;

namespace BaseFaq.Faq.Business.Faq.Abstractions;

public interface IFaqService
{
    Task<FaqDto> Create(FaqCreateRequestDto dto, CancellationToken token);
}