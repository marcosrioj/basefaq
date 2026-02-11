using MediatR;

namespace BaseFaq.Faq.Portal.Business.Faq.Commands.DeleteFaqContentRef;

public class FaqContentRefsDeleteFaqContentRefCommand : IRequest
{
    public required Guid Id { get; set; }
}