using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqItem;

namespace BaseFaq.Faq.Public.Business.FaqItem.Abstractions;

public interface IFaqItemService
{
    Task<Guid> Create(FaqItemCreateRequestDto requestDto, CancellationToken token);
    Task<PagedResultDto<FaqItemDto>> Search(FaqItemSearchRequestDto requestDto, CancellationToken token);
}