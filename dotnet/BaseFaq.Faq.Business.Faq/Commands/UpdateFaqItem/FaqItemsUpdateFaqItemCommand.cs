using BaseFaq.Models.Faq.Enums;
using MediatR;

namespace BaseFaq.Faq.Business.Faq.Commands.UpdateFaqItem;

public class FaqItemsUpdateFaqItemCommand : IRequest
{
    public required Guid Id { get; set; }
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public required FaqItemOrigin Origin { get; set; }
    public required int Sort { get; set; }
    public required int VoteScore { get; set; }
    public required int AiConfidenceScore { get; set; }
    public required bool IsActive { get; set; }
    public required Guid FaqId { get; set; }
}