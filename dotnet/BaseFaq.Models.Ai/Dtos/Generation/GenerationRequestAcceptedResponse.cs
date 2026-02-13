namespace BaseFaq.Models.Ai.Dtos.Generation;

public sealed record GenerationRequestAcceptedResponse(Guid CorrelationId, DateTime QueuedUtc);