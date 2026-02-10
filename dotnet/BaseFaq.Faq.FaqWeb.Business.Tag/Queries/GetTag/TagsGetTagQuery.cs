using BaseFaq.Models.Faq.Dtos.Tag;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Tag.Queries.GetTag;

public class TagsGetTagQuery : IRequest<TagDto?>
{
    public required Guid Id { get; set; }
}