using BaseFaq.Models.Faq.Enums;

namespace BaseFaq.Models.Faq.Dtos.Faq;

public class FaqCreateRequestDto
{
    public required string Name { get; set; }
    public required string Language { get; set; }
    public required FaqStatus Status { get; set; }
    public required FaqSortStrategy SortStrategy { get; set; }
    public bool CtaEnabled { get; set; }
    public CtaTarget CtaTarget { get; set; } = CtaTarget.Self;
}