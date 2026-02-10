using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Tag.Commands.UpdateTag;

public class TagsUpdateTagCommand : IRequest
{
    public required Guid Id { get; set; }
    public required string Value { get; set; }
}