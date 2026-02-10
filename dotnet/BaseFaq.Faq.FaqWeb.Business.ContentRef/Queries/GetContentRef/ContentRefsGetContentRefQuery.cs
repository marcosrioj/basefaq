using BaseFaq.Models.Faq.Dtos.ContentRef;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.ContentRef.Queries.GetContentRef;

public class ContentRefsGetContentRefQuery : IRequest<ContentRefDto?>
{
    public required Guid Id { get; set; }
}