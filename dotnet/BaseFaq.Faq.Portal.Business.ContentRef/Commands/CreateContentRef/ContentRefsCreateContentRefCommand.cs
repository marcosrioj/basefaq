using BaseFaq.Models.Faq.Enums;
using MediatR;

namespace BaseFaq.Faq.Portal.Business.ContentRef.Commands.CreateContentRef;

public class ContentRefsCreateContentRefCommand : IRequest<Guid>
{
    public required ContentRefKind Kind { get; set; }
    public required string Locator { get; set; }
    public string? Label { get; set; }
    public string? Scope { get; set; }
}