using BaseFaq.Models.Ai.Dtos.Generation;

namespace BaseFaq.AI.Generation.Business.Generation.Abstractions;

public interface IGenerationRequestService
{
    Task<GenerationRequestAcceptedResponse> EnqueueAsync(
        GenerationRequestDto request,
        string? idempotencyKey,
        CancellationToken token);
}