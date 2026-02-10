using BaseFaq.Models.Faq.Enums;

namespace BaseFaq.Models.Faq.Dtos.Faq;

public class FaqDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Language { get; set; }
    public required FaqStatus Status { get; set; }
    public required FaqSortStrategy SortStrategy { get; set; }
    public required bool CtaEnabled { get; set; }
    public required CtaTarget CtaTarget { get; set; }

    public required Guid TenantId { get; set; }
}