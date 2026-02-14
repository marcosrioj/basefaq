using BaseFaq.Models.Ai.Dtos.Generation;
using MediatR;

namespace BaseFaq.AI.Generation.Business.Generation.Queries.GetGenerationStatus;

public sealed class
    GenerationGetStatusQueryHandler : IRequestHandler<GenerationGetStatusQuery, GenerationStatusResponse>
{
    public Task<GenerationStatusResponse> Handle(
        GenerationGetStatusQuery request,
        CancellationToken cancellationToken)
    {
        var response = new GenerationStatusResponse(
            "generation",
            "ready",
            DateTime.UtcNow);

        return Task.FromResult(response);
    }
}