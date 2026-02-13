namespace BaseFaq.Models.Ai.Dtos.Matching;

public sealed record MatchingRequestDto(
    Guid TenantId,
    Guid FaqItemId,
    string Query,
    string Language,
    string? IdempotencyKey,
    Guid? RequestedByUserId);