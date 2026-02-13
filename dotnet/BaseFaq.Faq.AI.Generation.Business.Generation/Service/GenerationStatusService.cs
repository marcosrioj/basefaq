using BaseFaq.Faq.AI.Generation.Business.Generation.Abstractions;

namespace BaseFaq.Faq.AI.Generation.Business.Generation.Service;

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