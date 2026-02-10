using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Tag.Commands.CreateTag;

public class TagsCreateTagCommand : IRequest<Guid>
{
    public required string Value { get; set; }
    public required Guid TenantId { get; set; }
}