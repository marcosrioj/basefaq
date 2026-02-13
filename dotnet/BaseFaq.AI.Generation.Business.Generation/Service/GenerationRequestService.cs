using BaseFaq.AI.Common.Contracts.Generation;
using BaseFaq.AI.Generation.Business.Generation.Abstractions;
using MassTransit;

namespace BaseFaq.AI.Generation.Business.Generation.Service;

public sealed class GenerationRequestService(IPublishEndpoint publishEndpoint) : IGenerationRequestService
{
    public async Task<GenerationRequestAcceptedResponse> EnqueueAsync(GenerationRequestDto request,
        CancellationToken token)
    {
        var queuedUtc = DateTime.UtcNow;
        var correlationId = Guid.NewGuid();

        var message = new FaqGenerationRequestedV1
        {
            CorrelationId = correlationId,
            FaqId = request.FaqId,
            RequestedByUserId = request.RequestedByUserId ?? Guid.Empty,
            Language = request.Language,
            PromptProfile = request.PromptProfile,
            IdempotencyKey = request.IdempotencyKey ?? correlationId.ToString("N"),
            RequestedUtc = queuedUtc
        };

        await publishEndpoint.Publish(message, token);

        return new GenerationRequestAcceptedResponse(correlationId, queuedUtc);
    }
}