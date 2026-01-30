using BaseFaq.Models.Faq.Dtos.FaqItem;
using MediatR;

namespace BaseFaq.Faq.Business.Faq.Queries.GetFaqItem;

public class FaqItemsGetFaqItemQuery : IRequest<FaqItemDto?>
{
    public required Guid Id { get; set; }
}