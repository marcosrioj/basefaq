using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.ContentRef.Commands.DeleteContentRef;

public class ContentRefsDeleteContentRefCommand : IRequest
{
    public required Guid Id { get; set; }
}