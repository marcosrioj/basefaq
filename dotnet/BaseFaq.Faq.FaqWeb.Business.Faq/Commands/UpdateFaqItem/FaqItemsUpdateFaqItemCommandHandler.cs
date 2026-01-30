using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Repositories;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.UpdateFaqItem;

public class FaqItemsUpdateFaqItemCommandHandler(IFaqItemRepository faqItemRepository)
    : IRequestHandler<FaqItemsUpdateFaqItemCommand>
{
    public async Task Handle(FaqItemsUpdateFaqItemCommand request, CancellationToken cancellationToken)
    {
        var faqItem = await faqItemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (faqItem is null)
        {
            throw new KeyNotFoundException($"FAQ item '{request.Id}' was not found.");
        }

        faqItem.Question = request.Question;
        faqItem.Answer = request.Answer;
        faqItem.Origin = request.Origin;
        faqItem.Sort = request.Sort;
        faqItem.VoteScore = request.VoteScore;
        faqItem.AiConfidenceScore = request.AiConfidenceScore;
        faqItem.IsActive = request.IsActive;
        faqItem.FaqId = request.FaqId;

        faqItemRepository.Update(faqItem);
        await faqItemRepository.SaveChangesAsync(cancellationToken);
    }
}