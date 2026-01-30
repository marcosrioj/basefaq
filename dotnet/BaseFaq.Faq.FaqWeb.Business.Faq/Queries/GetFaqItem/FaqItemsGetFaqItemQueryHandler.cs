using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Repositories;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqItem;

public class FaqItemsGetFaqItemQueryHandler(IFaqItemRepository faqItemRepository)
    : IRequestHandler<FaqItemsGetFaqItemQuery, FaqItemDto?>
{
    public async Task<FaqItemDto?> Handle(FaqItemsGetFaqItemQuery request, CancellationToken cancellationToken)
    {
        var faqItem = await faqItemRepository.GetByIdAsync(request.Id, cancellationToken);
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