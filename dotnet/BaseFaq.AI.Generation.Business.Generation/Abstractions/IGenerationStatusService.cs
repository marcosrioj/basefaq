using BaseFaq.Models.Ai.Dtos.Generation;

namespace BaseFaq.AI.Generation.Business.Generation.Abstractions;

public interface IGenerationStatusService
{
    Task<GenerationStatusResponse> GetStatusAsync(CancellationToken token);
}