namespace BaseFaq.AI.Common.Contracts.Generation;

public sealed record FaqGenerationFailedV1
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required Guid JobId { get; init; }
    public required Guid FaqId { get; init; }
    public required string ErrorCode { get; init; }
    public required string ErrorMessage { get; init; }
    public required DateTime OccurredUtc { get; init; }
}