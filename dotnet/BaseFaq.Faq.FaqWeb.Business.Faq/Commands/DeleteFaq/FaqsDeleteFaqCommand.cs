using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.DeleteFaq;

public class FaqsDeleteFaqCommand : IRequest
{
    public required Guid Id { get; set; }
}