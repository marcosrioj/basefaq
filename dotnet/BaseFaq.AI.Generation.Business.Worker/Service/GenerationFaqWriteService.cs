using BaseFaq.AI.Generation.Business.Worker.Abstractions;
using BaseFaq.Faq.Common.Persistence.FaqDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BaseFaq.AI.Generation.Business.Worker.Service;

public sealed class GenerationFaqWriteService(
    IFaqIntegrationDbContextFactory faqIntegrationDbContextFactory,
    ILogger<GenerationFaqWriteService> logger) : IGenerationFaqWriteService
{
    public async Task WriteAsync(GenerationFaqWriteRequest request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        await using var faqDbContext = faqIntegrationDbContextFactory.Create(request.TenantId);

        var faq = await faqDbContext.Faqs
            .FirstOrDefaultAsync(x => x.Id == request.FaqId, cancellationToken);

        if (faq is null)
        {
            throw new InvalidOperationException(
                $"FAQ '{request.FaqId}' was not found for tenant '{request.TenantId}'.");
        }

        var itemId =
            GenerationFaqWriteIdempotencyHelper.CreateDeterministicFaqItemId(
                request.CorrelationId,
                request.FaqId,
                request.TenantId);

        var faqItem = await faqDbContext.FaqItems
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.Id == itemId && x.FaqId == request.FaqId && x.TenantId == request.TenantId,
                cancellationToken);

        if (faqItem is null)
        {
            faqItem = new FaqItem
            {
                Id = itemId,
                TenantId = request.TenantId,
                FaqId = request.FaqId,
                Question = request.Question,
                ShortAnswer = request.ShortAnswer,
                Answer = request.Answer,
                AdditionalInfo = request.AdditionalInfo,
                CtaTitle = request.CtaTitle,
                CtaUrl = request.CtaUrl,
                VoteScore = 0,
                AiConfidenceScore = request.AiConfidenceScore,
                IsActive = true,
                Sort = await GetNextSortAsync(faqDbContext, request, cancellationToken)
            };

            faqDbContext.FaqItems.Add(faqItem);
        }
        else
        {
            faqItem.Question = request.Question;
            faqItem.ShortAnswer = request.ShortAnswer;
            faqItem.Answer = request.Answer;
            faqItem.AdditionalInfo = request.AdditionalInfo;
            faqItem.CtaTitle = request.CtaTitle;
            faqItem.CtaUrl = request.CtaUrl;
            faqItem.AiConfidenceScore = request.AiConfidenceScore;
            faqItem.IsActive = true;
            faqItem.IsDeleted = false;
            faqItem.DeletedDate = null;
            faqItem.DeletedBy = null;
        }

        await faqDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "FAQ integration write completed for CorrelationId {CorrelationId}, FaqId {FaqId}, TenantId {TenantId}.",
            request.CorrelationId,
            request.FaqId,
            request.TenantId);
    }

    private static async Task<int> GetNextSortAsync(
        BaseFaq.Faq.Common.Persistence.FaqDb.FaqDbContext faqDbContext,
        GenerationFaqWriteRequest request,
        CancellationToken cancellationToken)
    {
        var maxSort = await faqDbContext.FaqItems
            .Where(x => x.FaqId == request.FaqId && !x.IsDeleted)
            .Select(x => (int?)x.Sort)
            .MaxAsync(cancellationToken);

        return (maxSort ?? 0) + 1;
    }

    private static void ValidateRequest(GenerationFaqWriteRequest request)
    {
        if (request.CorrelationId == Guid.Empty)
        {
            throw new ArgumentException("CorrelationId is required.", nameof(request));
        }

        if (request.FaqId == Guid.Empty)
        {
            throw new ArgumentException("FaqId is required.", nameof(request));
        }

        if (request.TenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId is required.", nameof(request));
        }

        ValidateRequired(request.Question, FaqItem.MaxQuestionLength, nameof(request.Question));
        ValidateRequired(request.ShortAnswer, FaqItem.MaxShortAnswerLength, nameof(request.ShortAnswer));
        ValidateOptional(request.Answer, FaqItem.MaxAnswerLength, nameof(request.Answer));
        ValidateOptional(request.AdditionalInfo, FaqItem.MaxAdditionalInfoLength, nameof(request.AdditionalInfo));
        ValidateOptional(request.CtaTitle, FaqItem.MaxCtaTitleLength, nameof(request.CtaTitle));
        ValidateOptional(request.CtaUrl, FaqItem.MaxCtaUrlLength, nameof(request.CtaUrl));

        if (request.AiConfidenceScore is < 0 or > 100)
        {
            throw new ArgumentException("AiConfidenceScore must be between 0 and 100.",
                nameof(request.AiConfidenceScore));
        }
    }

    private static void ValidateRequired(string? value, int maxLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        if (value.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must have at most {maxLength} characters.", fieldName);
        }
    }

    private static void ValidateOptional(string? value, int maxLength, string fieldName)
    {
        if (value is null)
        {
            return;
        }

        if (value.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must have at most {maxLength} characters.", fieldName);
        }
    }
}