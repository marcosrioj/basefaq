using BaseFaq.Faq.Common.Persistence.FaqDb.Base;
using BaseFaq.Models.Enums;

namespace BaseFaq.Faq.Common.Persistence.FaqDb.Entities;

public class FaqItem : BaseEntity
{
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public required FaqItemOrigin Origin { get; set; }
    public int Sort { get; set; }
    public int VoteScore { get; set; }
    public int AiConfidenceScore { get; set; }
    public bool IsActive { get; set; }

    public required Guid FaqId { get; set; }
}