namespace BaseFaq.AI.Generation.Business.Worker.Abstractions;

public interface IGenerationFaqWriteService
{
    Task WriteAsync(GenerationFaqWriteRequest request, CancellationToken cancellationToken);
}

public sealed record GenerationFaqWriteRequest(
    Guid CorrelationId,
    Guid FaqId,
    Guid TenantId,
    string Question,
    string ShortAnswer,
    string? Answer,
    string? AdditionalInfo,
    string? CtaTitle,
    string? CtaUrl,
    int AiConfidenceScore);