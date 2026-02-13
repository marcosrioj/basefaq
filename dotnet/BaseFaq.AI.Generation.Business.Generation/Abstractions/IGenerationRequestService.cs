namespace BaseFaq.AI.Generation.Business.Generation.Abstractions;

public interface IGenerationRequestService
{
    Task<GenerationRequestAcceptedResponse> EnqueueAsync(GenerationRequestDto request, CancellationToken token);
}

public sealed record GenerationRequestDto(
    Guid FaqId,
    string Language,
    string PromptProfile,
    string? IdempotencyKey,
    Guid? RequestedByUserId);

public sealed record GenerationRequestAcceptedResponse(Guid CorrelationId, DateTime QueuedUtc);