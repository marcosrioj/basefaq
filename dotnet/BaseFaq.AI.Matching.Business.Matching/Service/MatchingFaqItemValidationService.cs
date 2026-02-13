using BaseFaq.AI.Matching.Business.Matching.Abstractions;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.AI.Matching.Business.Matching.Service;

public sealed class MatchingFaqItemValidationService(FaqDbContext faqDbContext) : IMatchingFaqItemValidationService
{
    public async Task EnsureFaqItemExistsAsync(Guid faqItemId, CancellationToken token)
    {
        var exists = await faqDbContext.FaqItems
            .AsNoTracking()
            .AnyAsync(x => x.Id == faqItemId, token);

        if (!exists)
        {
            throw new ArgumentException(
                "FaqItemId does not exist or is not accessible for the current tenant.",
                nameof(faqItemId));
        }
    }
}