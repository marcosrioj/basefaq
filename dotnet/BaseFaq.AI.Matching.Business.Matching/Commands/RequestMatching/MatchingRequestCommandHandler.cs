using BaseFaq.AI.Matching.Business.Matching.Abstractions;
using BaseFaq.Models.Ai.Dtos.Matching;
using MediatR;

namespace BaseFaq.AI.Matching.Business.Matching.Commands.RequestMatching;

public sealed class MatchingRequestCommandHandler(
    IMatchingFaqItemValidationService faqItemValidationService,
    IMatchingRequestPublisher matchingRequestPublisher)
    : IRequestHandler<MatchingRequestCommand, Guid>
{
    private const int MaxQueryLength = 2000;
    private const int MaxLanguageLength = 16;
    private const int MaxIdempotencyKeyLength = 128;

    public async Task<Guid> Handle(
        MatchingRequestCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.Request);

        var normalizedIdempotencyKey = ValidateRequest(command.Request, command.IdempotencyKey);
        await faqItemValidationService.EnsureFaqItemExistsAsync(command.Request.FaqItemId, cancellationToken);

        var queuedUtc = DateTime.UtcNow;
        var correlationId = Guid.NewGuid();

        await matchingRequestPublisher.PublishAsync(
            command.Request,
            normalizedIdempotencyKey,
            correlationId,
            queuedUtc,
            cancellationToken);

        return correlationId;
    }

    private static string ValidateRequest(MatchingRequestDto request, string? idempotencyKey)
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

        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            throw new ArgumentException("Idempotency-Key header is required.", nameof(idempotencyKey));
        }

        var normalizedIdempotencyKey = idempotencyKey.Trim();
        if (normalizedIdempotencyKey.Length > MaxIdempotencyKeyLength)
        {
            throw new ArgumentException(
                $"IdempotencyKey must have at most {MaxIdempotencyKeyLength} characters.",
                nameof(idempotencyKey));
        }

        if (!string.IsNullOrWhiteSpace(request.IdempotencyKey) &&
            !string.Equals(request.IdempotencyKey, normalizedIdempotencyKey, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "Request body idempotency key does not match Idempotency-Key header.",
                nameof(request));
        }

        return normalizedIdempotencyKey;
    }
}