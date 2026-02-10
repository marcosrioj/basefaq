using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.UpdateFaqContentRef;

public class FaqContentRefsUpdateFaqContentRefCommand : IRequest
{
    public required Guid Id { get; set; }
    public required Guid FaqId { get; set; }
    public required Guid ContentRefId { get; set; }
}