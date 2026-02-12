using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Models.Faq.Dtos.ContentRef;
using BaseFaq.Models.Faq.Dtos.Faq;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using BaseFaq.Models.Faq.Dtos.Tag;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.Public.Business.Faq.Queries.GetFaq;

public class FaqsGetFaqQueryHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqsGetFaqQuery, FaqDetailDto?>
{
    public async Task<FaqDetailDto?> Handle(FaqsGetFaqQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Request);

        var query = dbContext.Faqs.AsNoTracking().Where(faq => faq.Id == request.Id);

        var includeItems = request.Request.IncludeFaqItems;
        var includeContentRefs = request.Request.IncludeContentRefs;
        var includeTags = request.Request.IncludeTags;

        if (includeItems)
        {
            query = query.Include(faq => faq.Items);
        }

        if (includeContentRefs)
        {
            query = query.Include(faq => faq.ContentRefs).ThenInclude(faqContentRef => faqContentRef.ContentRef);
        }

        if (includeTags)
        {
            query = query.Include(faq => faq.Tags).ThenInclude(faqTag => faqTag.Tag);
        }

        return await query
            .Select(faq => new FaqDetailDto
            {
                Id = faq.Id,
                Name = faq.Name,
                Language = faq.Language,
                Status = faq.Status,
                SortStrategy = faq.SortStrategy,
                CtaEnabled = faq.CtaEnabled,
                CtaTarget = faq.CtaTarget,
                TenantId = faq.TenantId,
                Items = includeItems
                    ? faq.Items.Select(item => new FaqItemDto
                    {
                        Id = item.Id,
                        Question = item.Question,
                        ShortAnswer = item.ShortAnswer,
                        Answer = item.Answer,
                        AdditionalInfo = item.AdditionalInfo,
                        CtaTitle = item.CtaTitle,
                        CtaUrl = item.CtaUrl,
                        Sort = item.Sort,
                        VoteScore = item.VoteScore,
                        AiConfidenceScore = item.AiConfidenceScore,
                        IsActive = item.IsActive,
                        FaqId = item.FaqId,
                        ContentRefId = item.ContentRefId
                    }).ToList()
                    : null,
                ContentRefs = includeContentRefs
                    ? faq.ContentRefs.Select(faqContentRef => new ContentRefDto
                    {
                        Id = faqContentRef.ContentRef.Id,
                        Kind = faqContentRef.ContentRef.Kind,
                        Locator = faqContentRef.ContentRef.Locator,
                        Label = faqContentRef.ContentRef.Label,
                        Scope = faqContentRef.ContentRef.Scope,
                        TenantId = faqContentRef.ContentRef.TenantId
                    }).ToList()
                    : null,
                Tags = includeTags
                    ? faq.Tags.Select(faqTag => new TagDto
                    {
                        Id = faqTag.Tag.Id,
                        Value = faqTag.Tag.Value,
                        TenantId = faqTag.Tag.TenantId
                    }).ToList()
                    : null
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}