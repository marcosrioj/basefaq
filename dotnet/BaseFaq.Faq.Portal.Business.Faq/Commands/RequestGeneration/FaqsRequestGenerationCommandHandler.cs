using System.Net;
using BaseFaq.AI.Common.Contracts.Generation;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Faq.Portal.Business.Faq.Dtos;
using BaseFaq.Models.Common.Enums;
using BaseFaq.Models.Faq.Enums;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BaseFaq.Faq.Portal.Business.Faq.Commands.RequestGeneration;

public sealed class FaqsRequestGenerationCommandHandler(
    FaqDbContext dbContext,
    ISessionService sessionService,
    IPublishEndpoint publishEndpoint,
    IConfiguration configuration) : IRequestHandler<FaqsRequestGenerationCommand, FaqGenerationRequestAcceptedDto>
{
    private static readonly ContentRefKind[] ProcessableContentRefKinds =
    [
        ContentRefKind.Web,
        ContentRefKind.Pdf,
        ContentRefKind.Document,
        ContentRefKind.Video
    ];

    private const int MaxLanguageLength = 16;
    private const int MaxPromptProfileLength = 128;
    private const string DefaultPromptProfile = "default";

    public async Task<FaqGenerationRequestAcceptedDto> Handle(
        FaqsRequestGenerationCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var faqId = request.FaqId;
        var tenantId = sessionService.GetTenantId(AppEnum.Faq);
        var userId = sessionService.GetUserId();

        var faq = await dbContext.Faqs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == faqId && x.TenantId == tenantId, cancellationToken);

        if (faq is null)
        {
            throw new ApiErrorException(
                $"FAQ '{faqId}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        if (string.IsNullOrWhiteSpace(faq.Language) || faq.Language.Length > MaxLanguageLength)
        {
            throw new ApiErrorException(
                $"FAQ language is required and must have at most {MaxLanguageLength} characters to request generation.",
                errorCode: (int)HttpStatusCode.BadRequest);
        }

        var contentRefs = await dbContext.FaqContentRefs
            .AsNoTracking()
            .Where(x => x.FaqId == faqId && x.TenantId == tenantId)
            .Select(x => x.ContentRef.Kind)
            .ToListAsync(cancellationToken);

        if (contentRefs.Count == 0)
        {
            throw new ApiErrorException(
                $"FAQ '{faqId}' must have at least one ContentRef to request generation.",
                errorCode: (int)HttpStatusCode.BadRequest);
        }

        var hasProcessableContentRef = contentRefs.Any(kind => ProcessableContentRefKinds.Contains(kind));
        if (!hasProcessableContentRef)
        {
            throw new ApiErrorException(
                $"FAQ '{faqId}' has ContentRef entries, but none with a processable kind (Web, Pdf, Document, Video).",
                errorCode: (int)HttpStatusCode.BadRequest);
        }

        var promptProfile = (configuration["Ai:Generation:PromptProfile"] ?? DefaultPromptProfile).Trim();
        if (string.IsNullOrWhiteSpace(promptProfile) || promptProfile.Length > MaxPromptProfileLength)
        {
            throw new ApiErrorException(
                $"PromptProfile must be between 1 and {MaxPromptProfileLength} characters.",
                errorCode: (int)HttpStatusCode.BadRequest);
        }

        var queuedUtc = DateTime.UtcNow;
        var correlationId = Guid.NewGuid();

        await publishEndpoint.Publish(new FaqGenerationRequestedV1
        {
            CorrelationId = correlationId,
            FaqId = faqId,
            TenantId = tenantId,
            RequestedByUserId = userId,
            Language = faq.Language,
            PromptProfile = promptProfile,
            IdempotencyKey = $"faq-generation-{faqId:N}-{correlationId:N}",
            RequestedUtc = queuedUtc
        }, cancellationToken);

        return new FaqGenerationRequestAcceptedDto(correlationId, queuedUtc);
    }
}