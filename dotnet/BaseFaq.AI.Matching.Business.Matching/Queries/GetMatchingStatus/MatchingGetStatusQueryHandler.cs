using BaseFaq.Models.Ai.Dtos.Matching;
using MediatR;

namespace BaseFaq.AI.Matching.Business.Matching.Queries.GetMatchingStatus;

public sealed class MatchingGetStatusQueryHandler : IRequestHandler<MatchingGetStatusQuery, MatchingStatusResponse>
{
    public Task<MatchingStatusResponse> Handle(
        MatchingGetStatusQuery request,
        CancellationToken cancellationToken)
    {
        var response = new MatchingStatusResponse(
            "matching",
            "ready",
            DateTime.UtcNow);

        return Task.FromResult(response);
    }
}