using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using MediatR;

namespace BaseFaq.Faq.Public.Business.FaqItem.Queries.SearchFaqItem;

public class FaqItemsSearchFaqItemQuery : IRequest<PagedResultDto<FaqItemDto>>
{
    public required FaqItemSearchRequestDto Request { get; set; }
}