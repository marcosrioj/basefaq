using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Models.Common.Enums;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.CreateFaq;

public class FaqsCreateFaqCommandHandler(FaqDbContext dbContext, ISessionService sessionService)
    : IRequestHandler<FaqsCreateFaqCommand, Guid>
{
    public async Task<Guid> Handle(FaqsCreateFaqCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = sessionService.GetTenantId(AppEnum.Faq);

        var faq = new FaqWeb.Persistence.FaqDb.Entities.Faq
        {
            Name = request.Name,
            Language = request.Language,
            Status = request.Status,
            SortStrategy = request.SortStrategy,
            CtaEnabled = request.CtaEnabled,
            CtaTarget = request.CtaTarget,
            TenantId = tenantId
        };

        await dbContext.Faqs.AddAsync(faq, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return faq.Id;
    }
}