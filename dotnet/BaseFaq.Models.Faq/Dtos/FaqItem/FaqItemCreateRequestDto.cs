using BaseFaq.Models.Faq.Enums;

namespace BaseFaq.Models.Faq.Dtos.FaqItem;

public class FaqItemCreateRequestDto
{
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public required FaqItemOrigin Origin { get; set; }
    public string? CtaText { get; set; }
    public string? CtaUrl { get; set; }
    public required int Sort { get; set; }
    public required int VoteScore { get; set; }
    public required int AiConfidenceScore { get; set; }
    public required bool IsActive { get; set; }
    public required Guid FaqId { get; set; }
}