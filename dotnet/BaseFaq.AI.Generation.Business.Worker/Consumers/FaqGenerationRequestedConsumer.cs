using BaseFaq.AI.Common.Contracts.Generation;
using BaseFaq.AI.Common.Persistence.AiDb;
using BaseFaq.AI.Common.Persistence.AiDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.AI.Generation.Business.Worker.Consumers;

public sealed class FaqGenerationRequestedConsumer(AiDbContext aiDbContext)
    : MassTransit.IConsumer<FaqGenerationRequestedV1>
{
    public async Task Consume(MassTransit.ConsumeContext<FaqGenerationRequestedV1> context)
    {
        var message = context.Message;

        var alreadyExists = await aiDbContext.GenerationJobs
            .AnyAsync(x => x.CorrelationId == message.CorrelationId || x.IdempotencyKey == message.IdempotencyKey,
                context.CancellationToken);

        if (alreadyExists)
        {
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
        await aiDbContext.SaveChangesAsync(context.CancellationToken);

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
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                OccurredUtc = DateTime.UtcNow
            }, context.CancellationToken);
        }
    }
}