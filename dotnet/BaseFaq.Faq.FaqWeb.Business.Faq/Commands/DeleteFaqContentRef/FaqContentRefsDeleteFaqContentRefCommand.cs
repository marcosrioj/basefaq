using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.DeleteFaqContentRef;

public class FaqContentRefsDeleteFaqContentRefCommand : IRequest
{
    public required Guid Id { get; set; }
}