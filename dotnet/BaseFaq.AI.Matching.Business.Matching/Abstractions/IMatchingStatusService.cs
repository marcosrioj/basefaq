using BaseFaq.Models.Ai.Dtos.Matching;

namespace BaseFaq.AI.Matching.Business.Matching.Abstractions;

public interface IMatchingStatusService
{
    Task<MatchingStatusResponse> GetStatusAsync(CancellationToken token);
}

public interface IMatchingRequestService
{
    Task<MatchingRequestAcceptedResponse> EnqueueAsync(
        MatchingRequestDto request,
        string? idempotencyKey,
        CancellationToken token);
}

public interface IMatchingFaqItemValidationService
{
    Task EnsureFaqItemExistsAsync(Guid faqItemId, CancellationToken token);
}

public interface IMatchingRequestPublisher
{
    Task PublishAsync(MatchingRequestDto request, string idempotencyKey, Guid correlationId, DateTime queuedUtc,
        CancellationToken token);
}