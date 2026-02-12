using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.Public.Business.FaqItem.Queries.GetFaqItem;

public class FaqItemsGetFaqItemQueryHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqItemsGetFaqItemQuery, FaqItemDto?>
{
    public async Task<FaqItemDto?> Handle(FaqItemsGetFaqItemQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.FaqItems
            .AsNoTracking()
            .Where(item => item.Id == request.Id)
            .Select(item => new FaqItemDto
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
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}