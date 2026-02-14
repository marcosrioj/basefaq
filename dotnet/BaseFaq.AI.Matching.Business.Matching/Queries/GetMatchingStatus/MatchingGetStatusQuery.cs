using BaseFaq.Models.Ai.Dtos.Matching;
using MediatR;

namespace BaseFaq.AI.Matching.Business.Matching.Queries.GetMatchingStatus;

public sealed record MatchingGetStatusQuery : IRequest<MatchingStatusResponse>;