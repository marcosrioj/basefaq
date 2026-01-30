using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Repositories;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.CreateFaqItem;

public class FaqItemsCreateFaqItemCommandHandler(IFaqItemRepository faqItemRepository)
    : IRequestHandler<FaqItemsCreateFaqItemCommand, Guid>
{
    public async Task<Guid> Handle(FaqItemsCreateFaqItemCommand request, CancellationToken cancellationToken)
    {
        var faqItem = new FaqItem
        {
            Question = request.Question,
            Answer = request.Answer,
            Origin = request.Origin,
            Sort = request.Sort,
            VoteScore = request.VoteScore,
            AiConfidenceScore = request.AiConfidenceScore,
            IsActive = request.IsActive,
            FaqId = request.FaqId
        };

        await faqItemRepository.AddAsync(faqItem, cancellationToken);
        await faqItemRepository.SaveChangesAsync(cancellationToken);

        return faqItem.Id;
    }
}