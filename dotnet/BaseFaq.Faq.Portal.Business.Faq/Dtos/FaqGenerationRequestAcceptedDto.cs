namespace BaseFaq.Faq.Portal.Business.Faq.Dtos;

public sealed record FaqGenerationRequestAcceptedDto(Guid CorrelationId, DateTime QueuedUtc);