using BaseFaq.AI.Generation.Business.Generation.Abstractions;
using BaseFaq.Models.Ai.Dtos.Generation;

namespace BaseFaq.AI.Generation.Business.Generation.Service;

public sealed class GenerationStatusService : IGenerationStatusService
{
    public Task<GenerationStatusResponse> GetStatusAsync(CancellationToken token)
    {
        var response = new GenerationStatusResponse(
            "generation",
            "ready",
            DateTime.UtcNow);

        return Task.FromResult(response);
    }
}