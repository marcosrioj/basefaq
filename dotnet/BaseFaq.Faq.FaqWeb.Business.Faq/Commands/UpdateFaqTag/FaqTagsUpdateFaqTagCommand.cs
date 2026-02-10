using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.UpdateFaqTag;

public class FaqTagsUpdateFaqTagCommand : IRequest
{
    public required Guid Id { get; set; }
    public required Guid FaqId { get; set; }
    public required Guid TagId { get; set; }
}