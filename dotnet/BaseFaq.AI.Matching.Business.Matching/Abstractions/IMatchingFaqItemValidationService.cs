namespace BaseFaq.AI.Matching.Business.Matching.Abstractions;

public interface IMatchingFaqItemValidationService
{
    Task EnsureFaqItemExistsAsync(Guid faqItemId, CancellationToken token);
}