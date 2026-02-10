using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.CreateFaq;

public class FaqsCreateFaqCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqsCreateFaqCommand, Guid>
{
    public async Task<Guid> Handle(FaqsCreateFaqCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var faq = new FaqWeb.Persistence.FaqDb.Entities.Faq
        {
            Name = request.Name,
            Language = request.Language,
            Status = request.Status,
            SortStrategy = request.SortStrategy,
            CtaEnabled = request.CtaEnabled,
            CtaTarget = request.CtaTarget,
            TenantId = request.TenantId
        };

        await dbContext.Faqs.AddAsync(faq, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return faq.Id;
    }
}