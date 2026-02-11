using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Tag.Commands.DeleteTag;

public class TagsDeleteTagCommand : IRequest
{
    public required Guid Id { get; set; }
}