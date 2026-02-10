using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Models.Faq.Dtos.FaqContentRef;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqContentRef;

public class FaqContentRefsGetFaqContentRefQueryHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqContentRefsGetFaqContentRefQuery, FaqContentRefDto?>
{
    public async Task<FaqContentRefDto?> Handle(
        FaqContentRefsGetFaqContentRefQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.FaqContentRefs
            .AsNoTracking()
            .Where(entity => entity.Id == request.Id)
            .Select(faqContentRef => new FaqContentRefDto
            {
                Id = faqContentRef.Id,
                FaqId = faqContentRef.FaqId,
                ContentRefId = faqContentRef.ContentRefId,
                TenantId = faqContentRef.TenantId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}