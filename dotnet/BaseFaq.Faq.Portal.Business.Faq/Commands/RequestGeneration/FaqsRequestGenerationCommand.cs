using MediatR;

namespace BaseFaq.Faq.Portal.Business.Faq.Commands.RequestGeneration;

public sealed class FaqsRequestGenerationCommand : IRequest<Guid>
{
    public required Guid FaqId { get; set; }
}