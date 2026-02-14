using BaseFaq.Models.Ai.Dtos.Matching;

namespace BaseFaq.AI.Matching.Business.Matching.Abstractions;

public interface IMatchingRequestPublisher
{
    Task PublishAsync(
        MatchingRequestDto request,
        string idempotencyKey,
        Guid correlationId,
        DateTime queuedUtc,
        CancellationToken token);
}