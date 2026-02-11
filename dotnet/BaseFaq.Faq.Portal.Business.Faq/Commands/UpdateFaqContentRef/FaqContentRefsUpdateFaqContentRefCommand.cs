using MediatR;

namespace BaseFaq.Faq.Portal.Business.Faq.Commands.UpdateFaqContentRef;

public class FaqContentRefsUpdateFaqContentRefCommand : IRequest
{
    public required Guid Id { get; set; }
    public required Guid FaqId { get; set; }
    public required Guid ContentRefId { get; set; }
}