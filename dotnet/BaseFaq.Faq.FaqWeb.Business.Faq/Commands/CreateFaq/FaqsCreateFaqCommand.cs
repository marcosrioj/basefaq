using BaseFaq.Models.Faq.Enums;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.CreateFaq;

public class FaqsCreateFaqCommand : IRequest<Guid>
{
    public required string Name { get; set; }
    public required string Language { get; set; }
    public required FaqStatus Status { get; set; }
    public required FaqSortStrategy SortStrategy { get; set; }
    public bool CtaEnabled { get; set; }
    public CtaTarget CtaTarget { get; set; } = CtaTarget.Self;
    public List<Guid> TagIds { get; set; } = [];
    public List<Guid> ContentRefIds { get; set; } = [];

    public required Guid TenantId { get; set; }
}