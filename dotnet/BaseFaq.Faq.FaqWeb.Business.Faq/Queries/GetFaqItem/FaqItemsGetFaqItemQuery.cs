using BaseFaq.Models.Faq.Dtos.FaqItem;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqItem;

public class FaqItemsGetFaqItemQuery : IRequest<FaqItemDto?>
{
    public required Guid Id { get; set; }
}