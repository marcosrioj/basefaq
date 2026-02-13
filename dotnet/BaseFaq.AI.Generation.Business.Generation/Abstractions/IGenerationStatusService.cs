namespace BaseFaq.AI.Generation.Business.Generation.Abstractions;

public interface IGenerationStatusService
{
    Task<GenerationStatusResponse> GetStatusAsync(CancellationToken token);
}

public sealed record GenerationStatusResponse(string Service, string Status, DateTime UtcTimestamp);