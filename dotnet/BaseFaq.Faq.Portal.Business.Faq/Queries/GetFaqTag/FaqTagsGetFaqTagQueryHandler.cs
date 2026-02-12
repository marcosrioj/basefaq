using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Models.Faq.Dtos.FaqTag;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.Portal.Business.Faq.Queries.GetFaqTag;

public class FaqTagsGetFaqTagQueryHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqTagsGetFaqTagQuery, FaqTagDto?>
{
    public async Task<FaqTagDto?> Handle(FaqTagsGetFaqTagQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.FaqTags
            .AsNoTracking()
            .Where(entity => entity.Id == request.Id)
            .Select(faqTag => new FaqTagDto
            {
                Id = faqTag.Id,
                FaqId = faqTag.FaqId,
                TagId = faqTag.TagId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}