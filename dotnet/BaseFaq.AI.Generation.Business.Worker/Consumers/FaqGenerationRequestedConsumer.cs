using BaseFaq.AI.Common.Contracts.Generation;
using BaseFaq.AI.Common.Persistence.AiDb;
using BaseFaq.AI.Common.Persistence.AiDb.Entities;
using BaseFaq.AI.Generation.Business.Worker.Abstractions;
using BaseFaq.Models.Ai.Enums;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.AI.Generation.Business.Worker.Consumers;

public sealed class FaqGenerationRequestedConsumer(
    AiDbContext aiDbContext,
    IGenerationFaqWriteService generationFaqWriteService)
    : MassTransit.IConsumer<FaqGenerationRequestedV1>
{
    public async Task Consume(MassTransit.ConsumeContext<FaqGenerationRequestedV1> context)
    {
        var handlerName = nameof(FaqGenerationRequestedConsumer);
        var messageId = context.MessageId?.ToString("N") ?? context.Message.CorrelationId.ToString("N");

        var processedMessageExists = await aiDbContext.ProcessedMessages
            .AnyAsync(x => x.HandlerName == handlerName && x.MessageId == messageId, context.CancellationToken);

        if (processedMessageExists)
        {
            return;
        }

        var message = context.Message;

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
            Provider = "local-stub",
            Model = "stub-v1"
        };

        aiDbContext.GenerationJobs.Add(job);
        try
        {
            await aiDbContext.SaveChangesAsync(context.CancellationToken);
        }
        catch (DbUpdateException ex) when (IsDuplicateJobException(ex))
        {
            await MarkProcessedAsync(handlerName, messageId, context.CancellationToken);
            return;
        }

        try
        {
            job.Artifacts.Add(new GenerationArtifact
            {
                GenerationJobId = job.Id,
                ArtifactType = GenerationArtifactType.Draft,
                Sequence = 1,
                Content = $"Generated draft placeholder for FAQ {message.FaqId}",
                MetadataJson = "{}"
            });

            await generationFaqWriteService.WriteAsync(new GenerationFaqWriteRequest(
                    message.CorrelationId,
                    message.FaqId,
                    message.TenantId,
                    "Generated draft question",
                    $"Draft summary for FAQ {message.FaqId}",
                    $"Generated draft placeholder for FAQ {message.FaqId}",
                    null,
                    null,
                    null,
                    80),
                context.CancellationToken);

            job.Status = GenerationJobStatus.Succeeded;
            job.CompletedUtc = DateTime.UtcNow;
            job.ErrorCode = null;
            job.ErrorMessage = null;

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
}