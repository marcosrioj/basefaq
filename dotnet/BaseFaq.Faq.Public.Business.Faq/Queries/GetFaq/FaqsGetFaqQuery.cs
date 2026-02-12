using BaseFaq.Models.Faq.Dtos.Faq;
using MediatR;

namespace BaseFaq.Faq.Public.Business.Faq.Queries.GetFaq;

public class FaqsGetFaqQuery : IRequest<FaqDetailDto?>
{
    public required Guid Id { get; set; }
    public required FaqGetRequestDto Request { get; set; }
}