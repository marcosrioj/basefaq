using BaseFaq.Models.Faq.Dtos.FaqTag;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqTag;

public class FaqTagsGetFaqTagQuery : IRequest<FaqTagDto?>
{
    public required Guid Id { get; set; }
}