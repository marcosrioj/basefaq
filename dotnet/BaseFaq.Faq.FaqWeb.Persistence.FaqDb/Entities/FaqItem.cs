using BaseFaq.Common.EntityFramework.Core.Entities;
using BaseFaq.Models.Faq.Enums;

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;

public class FaqItem : BaseEntity
{
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public required FaqItemOrigin Origin { get; set; }
    public string? CtaText { get; set; }
    public string? CtaUrl { get; set; }

    public int Sort { get; set; }
    public int VoteScore { get; set; }
    public int AiConfidenceScore { get; set; }
    public bool IsActive { get; set; }

    public required Guid FaqId { get; set; }
}