using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.DeleteFaqTag;

public class FaqTagsDeleteFaqTagCommand : IRequest
{
    public required Guid Id { get; set; }
}