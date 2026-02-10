using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Models.Common.Enums;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.FaqItem.Commands.CreateFaqItem;

public class FaqItemsCreateFaqItemCommandHandler(FaqDbContext dbContext, ISessionService sessionService)
    : IRequestHandler<FaqItemsCreateFaqItemCommand, Guid>
{
    public async Task<Guid> Handle(FaqItemsCreateFaqItemCommand request, CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(AppEnum.FaqWeb);

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
            TenantId = tenantId
        };

        await dbContext.FaqItems.AddAsync(faqItem, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return faqItem.Id;
    }
}