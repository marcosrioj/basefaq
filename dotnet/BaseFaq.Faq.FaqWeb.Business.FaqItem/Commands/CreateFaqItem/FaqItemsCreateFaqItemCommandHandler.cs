using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.FaqItem.Commands.CreateFaqItem;

public class FaqItemsCreateFaqItemCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqItemsCreateFaqItemCommand, Guid>
{
    public async Task<Guid> Handle(FaqItemsCreateFaqItemCommand request, CancellationToken cancellationToken)
    {
        var faqItem = new Persistence.FaqDb.Entities.FaqItem
        {
            Question = request.Question,
            ShortAnswer = request.ShortAnswer,
            Answer = request.Answer,
            AdditionalInfo = request.AdditionalInfo,
            CtaTitle = request.CtaTitle,
            CtaUrl = request.CtaUrl,
            Sort = request.Sort,
            VoteScore = request.VoteScore,
            AiConfidenceScore = request.AiConfidenceScore,
            IsActive = request.IsActive,
            FaqId = request.FaqId,
            ContentRefId = request.ContentRefId,
            TenantId = request.TenantId
        };

        await dbContext.FaqItems.AddAsync(faqItem, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return faqItem.Id;
    }
}