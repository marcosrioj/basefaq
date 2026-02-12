using BaseFaq.Models.Faq.Dtos.ContentRef;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using BaseFaq.Models.Faq.Dtos.Tag;
using BaseFaq.Models.Faq.Enums;

namespace BaseFaq.Models.Faq.Dtos.Faq;

public class FaqDetailDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Language { get; set; }
    public required FaqStatus Status { get; set; }
    public required FaqSortStrategy SortStrategy { get; set; }
    public required bool CtaEnabled { get; set; }
    public required CtaTarget CtaTarget { get; set; }
    public required Guid TenantId { get; set; }

    public List<FaqItemDto>? Items { get; set; }
    public List<ContentRefDto>? ContentRefs { get; set; }
    public List<TagDto>? Tags { get; set; }
}