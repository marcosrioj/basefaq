namespace BaseFaq.Faq.AI.Matching.Business.Matching.Abstractions;

public interface IMatchingStatusService
{
    Task<MatchingStatusResponse> GetStatusAsync(CancellationToken token);
}

public sealed record MatchingStatusResponse(string Service, string Status, DateTime UtcTimestamp);