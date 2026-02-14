using BaseFaq.AI.Common.Contracts.Matching;
using BaseFaq.AI.Common.Providers.Abstractions;
using BaseFaq.AI.Common.Persistence.AiDb;
using BaseFaq.AI.Common.Persistence.AiDb.Entities;
using BaseFaq.AI.Matching.Business.Worker.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BaseFaq.AI.Matching.Business.Worker.Consumers;

public sealed class FaqMatchingRequestedConsumer(
    AiDbContext aiDbContext,
    IAiProviderCredentialAccessor aiProviderCredentialAccessor,
    IMatchingFaqDbContextFactory matchingFaqDbContextFactory) : MassTransit.IConsumer<FaqMatchingRequestedV1>
{
    private const int MaxCandidates = 5;
    private const string SimilarityErrorCode = "MATCHING_FAILED";
    private static readonly Regex WordSplitter = new("[^a-z0-9]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public async Task Consume(MassTransit.ConsumeContext<FaqMatchingRequestedV1> context)
    {
        var handlerName = nameof(FaqMatchingRequestedConsumer);
        var messageId = context.MessageId?.ToString("N") ?? context.Message.CorrelationId.ToString("N");
        var message = context.Message;

        var processedMessageExists = await aiDbContext.ProcessedMessages
            .AnyAsync(x => x.HandlerName == handlerName && x.MessageId == messageId, context.CancellationToken);

        if (processedMessageExists)
        {
            return;
        }

        try
        {
            _ = aiProviderCredentialAccessor.GetCurrent();
            using var faqDbContext = matchingFaqDbContextFactory.Create(message.TenantId);

            var source = await faqDbContext.FaqItems
                .AsNoTracking()
                .Where(x => x.Id == message.FaqItemId && x.TenantId == message.TenantId)
                .Select(x => new { x.Id, x.Question })
                .SingleOrDefaultAsync(context.CancellationToken);

            if (source is null)
            {
                throw new ArgumentException("FaqItemId does not exist for the tenant.", nameof(message.FaqItemId));
            }

            var queryText = string.IsNullOrWhiteSpace(message.Query) ? source.Question : message.Query;
            var candidates = await faqDbContext.FaqItems
                .AsNoTracking()
                .Where(x => x.TenantId == message.TenantId && x.Id != message.FaqItemId && x.IsActive)
                .Select(x => new { x.Id, x.Question })
                .ToListAsync(context.CancellationToken);

            var rankedCandidates = candidates
                .Select(x => new MatchingCandidate(x.Id, ComputeSimilarity(queryText, x.Question)))
                .Where(x => x.SimilarityScore > 0)
                .OrderByDescending(x => x.SimilarityScore)
                .Take(MaxCandidates)
                .ToArray();

            await context.Publish(new FaqMatchingCompletedV1
            {
                EventId = Guid.NewGuid(),
                CorrelationId = message.CorrelationId,
                TenantId = message.TenantId,
                FaqItemId = message.FaqItemId,
                Candidates = rankedCandidates,
                OccurredUtc = DateTime.UtcNow
            }, context.CancellationToken);

            await MarkProcessedAsync(handlerName, messageId, context.CancellationToken);
        }
        catch (Exception ex)
        {
            var errorMessage = ex.Message.Length <= GenerationJob.MaxErrorMessageLength
                ? ex.Message
                : ex.Message[..GenerationJob.MaxErrorMessageLength];

            await context.Publish(new FaqMatchingFailedV1
            {
                EventId = Guid.NewGuid(),
                CorrelationId = message.CorrelationId,
                TenantId = message.TenantId,
                FaqItemId = message.FaqItemId,
                ErrorCode = SimilarityErrorCode,
                ErrorMessage = errorMessage,
                OccurredUtc = DateTime.UtcNow
            }, context.CancellationToken);

            await MarkProcessedAsync(handlerName, messageId, context.CancellationToken);
        }
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
            // Duplicate dedupe key can happen with concurrent delivery handling.
        }
    }

    private static double ComputeSimilarity(string left, string right)
    {
        var leftTerms = Tokenize(left);
        var rightTerms = Tokenize(right);

        if (leftTerms.Count == 0 || rightTerms.Count == 0)
        {
            return 0;
        }

        var intersection = leftTerms.Intersect(rightTerms).Count();
        if (intersection == 0)
        {
            return 0;
        }

        var union = leftTerms.Union(rightTerms).Count();
        return Math.Round(intersection / (double)union, 4);
    }

    private static HashSet<string> Tokenize(string text)
    {
        return WordSplitter
            .Split(text.Trim().ToLowerInvariant())
            .Where(x => x.Length > 1)
            .ToHashSet(StringComparer.Ordinal);
    }
}