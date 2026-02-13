using BaseFaq.AI.Matching.Business.Matching.Abstractions;

namespace BaseFaq.AI.Matching.Business.Matching.Service;

public sealed class MatchingStatusService : IMatchingStatusService
{
    public Task<MatchingStatusResponse> GetStatusAsync(CancellationToken token)
    {
        var response = new MatchingStatusResponse(
            "matching",
            "ready",
            DateTime.UtcNow);

        return Task.FromResult(response);
    }
}