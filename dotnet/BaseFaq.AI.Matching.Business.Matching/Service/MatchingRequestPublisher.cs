using BaseFaq.AI.Common.Contracts.Matching;
using BaseFaq.AI.Matching.Business.Matching.Abstractions;
using BaseFaq.Models.Ai.Dtos.Matching;
using MassTransit;

namespace BaseFaq.AI.Matching.Business.Matching.Service;

public sealed class MatchingRequestPublisher(IPublishEndpoint publishEndpoint) : IMatchingRequestPublisher
{
    public async Task PublishAsync(
        MatchingRequestDto request,
        string idempotencyKey,
        Guid correlationId,
        DateTime queuedUtc,
        CancellationToken token)
    {
        var message = new FaqMatchingRequestedV1
        {
            CorrelationId = correlationId,
            TenantId = request.TenantId,
            FaqItemId = request.FaqItemId,
            RequestedByUserId = request.RequestedByUserId ?? Guid.Empty,
            Query = request.Query,
            Language = request.Language,
            IdempotencyKey = idempotencyKey,
            RequestedUtc = queuedUtc
        };

        await publishEndpoint.Publish(message, token);
    }
}