using BaseFaq.AI.Matching.Business.Matching.Abstractions;

namespace BaseFaq.AI.Matching.Business.Matching.Service;

public sealed class MatchingRequestService(IMatchingFaqItemValidationService faqItemValidationService)
    : IMatchingRequestService
{
    private const int MaxQueryLength = 2000;
    private const int MaxLanguageLength = 16;
    private const int MaxIdempotencyKeyLength = 128;

    public async Task<MatchingRequestAcceptedResponse> EnqueueAsync(MatchingRequestDto request, CancellationToken token)
    {
        ValidateRequest(request);
        await faqItemValidationService.EnsureFaqItemExistsAsync(request.FaqItemId, token);

        var queuedUtc = DateTime.UtcNow;
        var correlationId = Guid.NewGuid();

        return new MatchingRequestAcceptedResponse(correlationId, queuedUtc);
    }

    private static void ValidateRequest(MatchingRequestDto request)
    {
        if (request.TenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId is required.", nameof(request));
        }

        if (request.FaqItemId == Guid.Empty)
        {
            throw new ArgumentException("FaqItemId is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Query) || request.Query.Length > MaxQueryLength)
        {
            throw new ArgumentException(
                $"Query is required and must have at most {MaxQueryLength} characters.",
                nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Language) || request.Language.Length > MaxLanguageLength)
        {
            throw new ArgumentException(
                $"Language is required and must have at most {MaxLanguageLength} characters.",
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