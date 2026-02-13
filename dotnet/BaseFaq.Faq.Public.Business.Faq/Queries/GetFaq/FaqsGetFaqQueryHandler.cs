using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Constants;
using BaseFaq.Models.Faq.Dtos.ContentRef;
using BaseFaq.Models.Faq.Dtos.Faq;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using BaseFaq.Models.Faq.Dtos.Tag;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.Public.Business.Faq.Queries.GetFaq;

public class FaqsGetFaqQueryHandler(
    FaqDbContext dbContext,
    IClientKeyContextService clientKeyContextService,
    ITenantClientKeyResolver tenantClientKeyResolver,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<FaqsGetFaqQuery, FaqDetailDto?>
{
    public async Task<FaqDetailDto?> Handle(FaqsGetFaqQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Request);

        var clientKey = clientKeyContextService.GetRequiredClientKey();
        var tenantId = await tenantClientKeyResolver.ResolveTenantId(clientKey, cancellationToken);
        httpContextAccessor.HttpContext?.Items[TenantContextKeys.TenantIdItemKey] = tenantId;

        var query = dbContext.Faqs
            .AsNoTracking()
            .Where(faq => faq.TenantId == tenantId && faq.Id == request.Id);

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
                        Scope = faqContentRef.ContentRef.Scope
                    }).ToList()
                    : null,
                Tags = includeTags
                    ? faq.Tags.Select(faqTag => new TagDto
                    {
                        Id = faqTag.Tag.Id,
                        Value = faqTag.Tag.Value
                    }).ToList()
                    : null
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}