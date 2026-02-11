using MediatR;

namespace BaseFaq.Faq.Portal.Business.ContentRef.Commands.DeleteContentRef;

public class ContentRefsDeleteContentRefCommand : IRequest
{
    public required Guid Id { get; set; }
}