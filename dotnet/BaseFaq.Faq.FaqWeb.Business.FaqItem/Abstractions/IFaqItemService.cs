using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqItem;

namespace BaseFaq.Faq.FaqWeb.Business.FaqItem.Abstractions;

public interface IFaqItemService
{
    Task<FaqItemDto> Create(FaqItemCreateRequestDto dto, CancellationToken token);
    Task Delete(Guid id, CancellationToken token);
    Task<PagedResultDto<FaqItemDto>> GetAll(FaqItemGetAllRequestDto requestDto, CancellationToken token);
    Task<FaqItemDto> GetById(Guid id, CancellationToken token);
    Task<FaqItemDto> Update(Guid id, FaqItemUpdateRequestDto dto, CancellationToken token);
}