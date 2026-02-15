using BaseFaq.Faq.Portal.Business.Faq.Dtos;
using MediatR;

namespace BaseFaq.Faq.Portal.Business.Faq.Commands.RequestGeneration;

public sealed class FaqsRequestGenerationCommand : IRequest<FaqGenerationRequestAcceptedDto>
{
    public required Guid FaqId { get; set; }
}