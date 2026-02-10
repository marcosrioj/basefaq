using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.CreateFaqContentRef;

public class FaqContentRefsCreateFaqContentRefCommand : IRequest<Guid>
{
    public required Guid FaqId { get; set; }
    public required Guid ContentRefId { get; set; }
}