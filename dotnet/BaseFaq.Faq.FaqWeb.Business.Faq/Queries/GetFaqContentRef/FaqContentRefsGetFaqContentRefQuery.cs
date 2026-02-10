using BaseFaq.Models.Faq.Dtos.FaqContentRef;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqContentRef;

public class FaqContentRefsGetFaqContentRefQuery : IRequest<FaqContentRefDto?>
{
    public required Guid Id { get; set; }
}