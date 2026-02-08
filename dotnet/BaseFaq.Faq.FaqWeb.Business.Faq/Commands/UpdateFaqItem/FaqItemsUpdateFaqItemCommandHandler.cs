using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.UpdateFaqItem;

public class FaqItemsUpdateFaqItemCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqItemsUpdateFaqItemCommand>
{
    public async Task Handle(FaqItemsUpdateFaqItemCommand request, CancellationToken cancellationToken)
    {
        var faqItem = await dbContext.FaqItems.FirstOrDefaultAsync(
            entity => entity.Id == request.Id,
            cancellationToken);
        if (faqItem is null)
        {
            throw new ApiErrorException(
                $"FAQ item '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        faqItem.Question = request.Question;
        faqItem.Answer = request.Answer;
        faqItem.Origin = request.Origin;
        faqItem.CtaText = request.CtaText;
        faqItem.CtaUrl = request.CtaUrl;
        faqItem.Sort = request.Sort;
        faqItem.VoteScore = request.VoteScore;
        faqItem.AiConfidenceScore = request.AiConfidenceScore;
        faqItem.IsActive = request.IsActive;
        faqItem.FaqId = request.FaqId;

        dbContext.FaqItems.Update(faqItem);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}