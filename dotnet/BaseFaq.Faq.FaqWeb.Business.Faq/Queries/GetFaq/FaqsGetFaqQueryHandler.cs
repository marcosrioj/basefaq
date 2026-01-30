using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Models.Faq.Dtos.Faq;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaq;

public class FaqsGetFaqQueryHandler(FaqDbContext dbContext) : IRequestHandler<FaqsGetFaqQuery, FaqDto?>
{
    public async Task<FaqDto?> Handle(FaqsGetFaqQuery request, CancellationToken cancellationToken)
    {
        var faq = await dbContext.Faqs
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (faq is null)
        {
            return null;
        }

        return new FaqDto
        {
            Id = faq.Id,
            Name = faq.Name,
            Language = faq.Language,
            Status = faq.Status,
            SortType = faq.SortType,
            TenantId = faq.TenantId
        };
    }
}