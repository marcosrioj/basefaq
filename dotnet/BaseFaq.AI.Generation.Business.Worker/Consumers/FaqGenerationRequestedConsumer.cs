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
            CompletedUtc = now,
            Status = GenerationJobStatus.Succeeded,
            Provider = "local-stub",
            Model = "stub-v1"
        };

        job.Artifacts.Add(new GenerationArtifact
        {
            GenerationJobId = job.Id,
            ArtifactType = GenerationArtifactType.Draft,
            Sequence = 1,
            Content = $"Generated draft placeholder for FAQ {message.FaqId}",
            MetadataJson = "{}"
        });

        aiDbContext.GenerationJobs.Add(job);
        await aiDbContext.SaveChangesAsync(context.CancellationToken);
    }
}