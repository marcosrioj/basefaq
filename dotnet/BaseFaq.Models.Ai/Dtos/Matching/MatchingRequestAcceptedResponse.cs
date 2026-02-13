namespace BaseFaq.Models.Ai.Dtos.Matching;

public sealed record MatchingRequestAcceptedResponse(Guid CorrelationId, DateTime QueuedUtc);