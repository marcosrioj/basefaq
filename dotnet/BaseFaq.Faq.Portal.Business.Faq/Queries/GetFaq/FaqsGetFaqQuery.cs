using BaseFaq.Models.Faq.Dtos.Faq;
using MediatR;

namespace BaseFaq.Faq.Portal.Business.Faq.Queries.GetFaq;

public class FaqsGetFaqQuery : IRequest<FaqDto?>
{
    public required Guid Id { get; set; }
}