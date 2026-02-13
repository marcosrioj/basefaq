using BaseFaq.AI.Common.Contracts.Generation;
using BaseFaq.AI.Generation.Business.Generation.Abstractions;
using MassTransit;

namespace BaseFaq.AI.Generation.Business.Generation.Service;

public sealed class GenerationRequestService(IPublishEndpoint publishEndpoint) : IGenerationRequestService
{
    private const int MaxLanguageLength = 16;
    private const int MaxPromptProfileLength = 128;
    private const int MaxIdempotencyKeyLength = 128;

    public async Task<GenerationRequestAcceptedResponse> EnqueueAsync(GenerationRequestDto request,
        CancellationToken token)
    {
        ValidateRequest(request);

        var queuedUtc = DateTime.UtcNow;
        var correlationId = Guid.NewGuid();

        var message = new FaqGenerationRequestedV1
        {
            CorrelationId = correlationId,
            FaqId = request.FaqId,
            TenantId = request.TenantId,
            RequestedByUserId = request.RequestedByUserId ?? Guid.Empty,
            Language = request.Language,
            PromptProfile = request.PromptProfile,
            IdempotencyKey = request.IdempotencyKey ?? correlationId.ToString("N"),
            RequestedUtc = queuedUtc
        };

        await publishEndpoint.Publish(message, token);

        return new GenerationRequestAcceptedResponse(correlationId, queuedUtc);
    }

    private static void ValidateRequest(GenerationRequestDto request)
    {
        if (request.FaqId == Guid.Empty)
        {
            throw new ArgumentException("FaqId is required.", nameof(request));
        }

        if (request.TenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Language) || request.Language.Length > MaxLanguageLength)
        {
            throw new ArgumentException(
                $"Language is required and must have at most {MaxLanguageLength} characters.",
                nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.PromptProfile) ||
            request.PromptProfile.Length > MaxPromptProfileLength)
        {
            throw new ArgumentException(
                $"PromptProfile is required and must have at most {MaxPromptProfileLength} characters.",
                nameof(request));
        }

        if (!string.IsNullOrWhiteSpace(request.IdempotencyKey) &&
            request.IdempotencyKey.Length > MaxIdempotencyKeyLength)
        {
            throw new ArgumentException(
                $"IdempotencyKey must have at most {MaxIdempotencyKeyLength} characters.",
                nameof(request));
        }
    }
}