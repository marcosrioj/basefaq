using BaseFaq.Models.Faq.Dtos.Tag;
using MediatR;

namespace BaseFaq.Faq.Portal.Business.Tag.Queries.GetTag;

public class TagsGetTagQuery : IRequest<TagDto?>
{
    public required Guid Id { get; set; }
}