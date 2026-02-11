using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.FaqItem.Commands.DeleteFaqItem;

public class FaqItemsDeleteFaqItemCommand : IRequest
{
    public required Guid Id { get; set; }
}