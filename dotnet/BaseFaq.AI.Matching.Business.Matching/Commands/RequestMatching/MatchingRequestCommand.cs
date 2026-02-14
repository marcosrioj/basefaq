using BaseFaq.Models.Ai.Dtos.Matching;
using MediatR;

namespace BaseFaq.AI.Matching.Business.Matching.Commands.RequestMatching;

public sealed record MatchingRequestCommand(
    MatchingRequestDto Request,
    string? IdempotencyKey) : IRequest<MatchingRequestAcceptedResponse>;