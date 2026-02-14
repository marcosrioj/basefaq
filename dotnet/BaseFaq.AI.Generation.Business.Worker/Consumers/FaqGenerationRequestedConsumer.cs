using System.Diagnostics;
using BaseFaq.AI.Common.Contracts.Generation;
using BaseFaq.AI.Common.Persistence.AiDb;
using BaseFaq.AI.Common.Persistence.AiDb.Entities;
using BaseFaq.AI.Common.Providers.Abstractions;
using BaseFaq.AI.Generation.Business.Worker.Abstractions;
using BaseFaq.AI.Generation.Business.Worker.Observability;
using BaseFaq.AI.Generation.Business.Worker.Service;
using BaseFaq.Models.Ai.Enums;
using BaseFaq.Models.Faq.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BaseFaq.AI.Generation.Business.Worker.Consumers;

public sealed class FaqGenerationRequestedConsumer(
    AiDbContext aiDbContext,
    IAiProviderCredentialAccessor aiProviderCredentialAccessor,
    IFaqIntegrationDbContextFactory faqIntegrationDbContextFactory,
    IGenerationFaqWriteService generationFaqWriteService)
    : IConsumer<FaqGenerationRequestedV1>
{
    public async Task Consume(ConsumeContext<FaqGenerationRequestedV1> context)
    {
        using var consumeActivity =
            GenerationWorkerTracing.ActivitySource.StartActivity("generation.worker.consume",
                ActivityKind.Consumer);

        var handlerName = nameof(FaqGenerationRequestedConsumer);
        var messageId = context.MessageId?.ToString("N") ?? context.Message.CorrelationId.ToString("N");
        var message = context.Message;

        consumeActivity?.SetTag("messaging.system", "rabbitmq");
        consumeActivity?.SetTag("messaging.operation.name", "process");
        consumeActivity?.SetTag("messaging.message.id", messageId);
        consumeActivity?.SetTag("messaging.conversation_id", message.CorrelationId.ToString("D"));
        consumeActivity?.SetTag("basefaq.tenant_id", message.TenantId.ToString("D"));
        consumeActivity?.SetTag("basefaq.faq_id", message.FaqId.ToString("D"));

        var processedMessageExists = await aiDbContext.ProcessedMessages
            .AnyAsync(x => x.HandlerName == handlerName && x.MessageId == messageId, context.CancellationToken);

        if (processedMessageExists)
        {
            return;
        }

        var alreadyExists = await aiDbContext.GenerationJobs
            .AnyAsync(x =>
                    x.CorrelationId == message.CorrelationId ||
                    (x.FaqId == message.FaqId && x.IdempotencyKey == message.IdempotencyKey),
                context.CancellationToken);

        if (alreadyExists)
        {
            await MarkProcessedAsync(handlerName, messageId, context.CancellationToken);
            return;
        }

        var now = DateTime.UtcNow;
        var providerCredential = aiProviderCredentialAccessor.GetCurrent();

        var job = new GenerationJob
        {
            Id = Guid.NewGuid(),
            CorrelationId = message.CorrelationId,
            RequestedByUserId = message.RequestedByUserId,
            FaqId = message.FaqId,
            Language = message.Language,
            PromptProfile = message.PromptProfile,
            IdempotencyKey = message.IdempotencyKey,
            RequestedUtc = message.RequestedUtc,
            StartedUtc = now,
            Status = GenerationJobStatus.Processing,
            Provider = providerCredential.Provider,
            Model = providerCredential.Model
        };

        aiDbContext.GenerationJobs.Add(job);
        try
        {
            using var createJobActivity =
                GenerationWorkerTracing.ActivitySource.StartActivity("generation.ai_db.job_create",
                    ActivityKind.Internal);

            createJobActivity?.SetTag("db.system", "postgresql");
            createJobActivity?.SetTag("db.name", "bf_ai_db");

            await aiDbContext.SaveChangesAsync(context.CancellationToken);
        }
        catch (DbUpdateException ex) when (IsDuplicateJobException(ex))
        {
            await MarkProcessedAsync(handlerName, messageId, context.CancellationToken);
            return;
        }

        try
        {
            await using var faqDbContext = faqIntegrationDbContextFactory.Create(message.TenantId);
            var contentRefs = await faqDbContext.FaqContentRefs
                .AsNoTracking()
                .Where(x => x.FaqId == message.FaqId && x.TenantId == message.TenantId)
                .Select(x => new ContentRefStudyInput(x.ContentRef.Kind, x.ContentRef.Locator))
                .ToListAsync(context.CancellationToken);

            if (contentRefs.Count == 0)
            {
                throw new InvalidOperationException(
                    $"FAQ '{message.FaqId}' must have at least one ContentRef to continue generation.");
            }

            var studiedRefs = ContentRefStudyService.Study(contentRefs);

            using var providerActivity =
                GenerationWorkerTracing.ActivitySource.StartActivity("generation.provider.generate",
                    ActivityKind.Client);

            providerActivity?.SetTag("gen_ai.system", providerCredential.Provider);
            providerActivity?.SetTag("gen_ai.request.model", providerCredential.Model);
            providerActivity?.SetTag("basefaq.ai_key_slot", providerCredential.SelectedSlot);
            providerActivity?.SetTag("basefaq.ai_key_configured",
                !string.IsNullOrWhiteSpace(providerCredential.ApiKey));
            providerActivity?.SetTag("basefaq.correlation_id", message.CorrelationId.ToString("D"));
            providerActivity?.SetTag("basefaq.content_ref.total_count", studiedRefs.TotalCount);
            providerActivity?.SetTag("basefaq.content_ref.processed_count", studiedRefs.ProcessedCount);
            providerActivity?.SetTag("basefaq.content_ref.skipped_count", studiedRefs.SkippedCount);

            job.Artifacts.Add(new GenerationArtifact
            {
                GenerationJobId = job.Id,
                ArtifactType = GenerationArtifactType.Draft,
                Sequence = 1,
                Content = Truncate(BuildDraftContent(message.FaqId, studiedRefs), GenerationArtifact.MaxContentLength),
                MetadataJson = Truncate(
                    JsonSerializer.Serialize(new
                    {
                        contentRefTotal = studiedRefs.TotalCount,
                        contentRefProcessed = studiedRefs.ProcessedCount,
                        contentRefSkipped = studiedRefs.SkippedCount,
                        processedKinds = studiedRefs.StudiedRefs.Select(x => x.Kind.ToString()).ToArray()
                    }),
                    GenerationArtifact.MaxMetadataJsonLength)
            });

            await generationFaqWriteService.WriteAsync(new GenerationFaqWriteRequest(
                    message.CorrelationId,
                    message.FaqId,
                    message.TenantId,
                    Truncate(BuildDraftQuestion(studiedRefs), 1000),
                    Truncate(BuildDraftSummary(message.FaqId, studiedRefs), 250),
                    Truncate(BuildDraftContent(message.FaqId, studiedRefs), 5000),
                    null,
                    null,
                    null,
                    80),
                context.CancellationToken);

            job.Status = GenerationJobStatus.Succeeded;
            job.CompletedUtc = DateTime.UtcNow;
            job.ErrorCode = null;
            job.ErrorMessage = null;

            using var completeJobActivity =
                GenerationWorkerTracing.ActivitySource.StartActivity("generation.ai_db.job_complete",
                    ActivityKind.Internal);

            completeJobActivity?.SetTag("db.system", "postgresql");
            completeJobActivity?.SetTag("db.name", "bf_ai_db");

            await aiDbContext.SaveChangesAsync(context.CancellationToken);

            await context.Publish(new FaqGenerationReadyV1
            {
                EventId = Guid.NewGuid(),
                CorrelationId = message.CorrelationId,
                JobId = job.Id,
                FaqId = message.FaqId,
                TenantId = message.TenantId,
                OccurredUtc = DateTime.UtcNow
            }, context.CancellationToken);
        }
        catch (Exception ex)
        {
            job.Status = GenerationJobStatus.Failed;
            job.CompletedUtc = DateTime.UtcNow;
            const string errorCode = "GENERATION_FAILED";
            var errorMessage = ex.Message.Length <= GenerationJob.MaxErrorMessageLength
                ? ex.Message
                : ex.Message[..GenerationJob.MaxErrorMessageLength];
            job.ErrorCode = errorCode;
            job.ErrorMessage = errorMessage;

            using var failJobActivity =
                GenerationWorkerTracing.ActivitySource.StartActivity("generation.ai_db.job_fail",
                    ActivityKind.Internal);

            failJobActivity?.SetTag("db.system", "postgresql");
            failJobActivity?.SetTag("db.name", "bf_ai_db");
            failJobActivity?.SetTag("exception.type", ex.GetType().Name);

            await aiDbContext.SaveChangesAsync(context.CancellationToken);

            await context.Publish(new FaqGenerationFailedV1
            {
                EventId = Guid.NewGuid(),
                CorrelationId = message.CorrelationId,
                JobId = job.Id,
                FaqId = message.FaqId,
                TenantId = message.TenantId,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                OccurredUtc = DateTime.UtcNow
            }, context.CancellationToken);
        }

        await MarkProcessedAsync(handlerName, messageId, context.CancellationToken);
    }

    private async Task MarkProcessedAsync(string handlerName, string messageId, CancellationToken cancellationToken)
    {
        aiDbContext.ProcessedMessages.Add(new ProcessedMessage
        {
            HandlerName = handlerName,
            MessageId = messageId,
            ProcessedUtc = DateTime.UtcNow
        });

        try
        {
            await aiDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Another consumer execution may persist the same dedupe key first.
        }
    }

    private static bool IsDuplicateJobException(DbUpdateException ex)
    {
        var message = ex.InnerException?.Message ?? ex.Message;

        return message.Contains("IX_GenerationJob_CorrelationId", StringComparison.Ordinal) ||
               message.Contains("IX_GenerationJob_FaqId_IdempotencyKey", StringComparison.Ordinal);
    }

    private static string BuildDraftQuestion(ContentRefStudyResult studyResult)
    {
        if (studyResult.ProcessedCount == 0)
        {
            return "Generated draft question based on available content references";
        }

        var kinds = string.Join(", ", studyResult.StudiedRefs.Select(x => x.Kind.ToString()));
        return $"Generated draft question based on: {kinds}";
    }

    private static string BuildDraftSummary(Guid faqId, ContentRefStudyResult studyResult)
    {
        return
            $"Draft summary for FAQ {faqId}. ContentRefs total={studyResult.TotalCount}, processed={studyResult.ProcessedCount}, skipped={studyResult.SkippedCount}.";
    }

    private static string BuildDraftContent(Guid faqId, ContentRefStudyResult studyResult)
    {
        if (studyResult.ProcessedCount == 0)
        {
            return
                $"Generated draft placeholder for FAQ {faqId}. No processable ContentRef kind was found (all were skipped by business rules).";
        }

        var lines = studyResult.StudiedRefs
            .Select(x => $"{x.Kind} ({x.Locator}): {x.MainSubject}");

        return
            $"Generated draft placeholder for FAQ {faqId}. Source study:{Environment.NewLine}{string.Join(Environment.NewLine, lines)}";
    }

    private static string Truncate(string value, int maxLength)
    {
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}