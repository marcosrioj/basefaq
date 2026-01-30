using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqItem;

public class FaqItemsGetFaqItemQueryHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqItemsGetFaqItemQuery, FaqItemDto?>
{
    public async Task<FaqItemDto?> Handle(FaqItemsGetFaqItemQuery request, CancellationToken cancellationToken)
    {
        var faqItem = await dbContext.FaqItems
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (faqItem is null)
        {
            return null;
        }

        return new FaqItemDto
        {
            Id = faqItem.Id,
            Question = faqItem.Question,
            Answer = faqItem.Answer,
            Origin = faqItem.Origin,
            Sort = faqItem.Sort,
            VoteScore = faqItem.VoteScore,
            AiConfidenceScore = faqItem.AiConfidenceScore,
            IsActive = faqItem.IsActive,
            FaqId = faqItem.FaqId
        };
    }
}