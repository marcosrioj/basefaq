using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using MediatR;

namespace BaseFaq.Faq.Business.Faq.Queries.GetFaqItemList;

public class FaqItemsGetFaqItemListQuery : IRequest<PagedResultDto<FaqItemDto>>
{
    public required FaqItemGetAllRequestDto Request { get; set; }
}