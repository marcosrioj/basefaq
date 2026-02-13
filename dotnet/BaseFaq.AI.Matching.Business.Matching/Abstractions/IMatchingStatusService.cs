namespace BaseFaq.AI.Matching.Business.Matching.Abstractions;

public interface IMatchingStatusService
{
    Task<MatchingStatusResponse> GetStatusAsync(CancellationToken token);
}

public sealed record MatchingStatusResponse(string Service, string Status, DateTime UtcTimestamp);

public interface IMatchingRequestService
{
    Task<MatchingRequestAcceptedResponse> EnqueueAsync(MatchingRequestDto request, CancellationToken token);
}

public interface IMatchingFaqItemValidationService
{
    Task EnsureFaqItemExistsAsync(Guid faqItemId, CancellationToken token);
}

public sealed record MatchingRequestDto(
    Guid TenantId,
    Guid FaqItemId,
    string Query,
    string Language,
    string? IdempotencyKey,
    Guid? RequestedByUserId);

public sealed record MatchingRequestAcceptedResponse(Guid CorrelationId, DateTime QueuedUtc);