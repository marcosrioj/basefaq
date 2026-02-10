using BaseFaq.Models.Faq.Enums;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.UpdateFaq;

public class FaqsUpdateFaqCommand : IRequest
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Language { get; set; }
    public required FaqStatus Status { get; set; }
    public required FaqSortStrategy SortStrategy { get; set; }
    public bool CtaEnabled { get; set; }
    public CtaTarget CtaTarget { get; set; } = CtaTarget.Self;
    public List<Guid>? TagIds { get; set; }
    public List<Guid>? ContentRefIds { get; set; }
}