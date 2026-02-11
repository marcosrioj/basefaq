using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Models.Faq.Dtos.ContentRef;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.FaqWeb.Business.ContentRef.Queries.GetContentRef;

public class ContentRefsGetContentRefQueryHandler(FaqDbContext dbContext)
    : IRequestHandler<ContentRefsGetContentRefQuery, ContentRefDto?>
{
    public async Task<ContentRefDto?> Handle(ContentRefsGetContentRefQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.ContentRefs
            .AsNoTracking()
            .Where(entity => entity.Id == request.Id)
            .Select(contentRef => new ContentRefDto
            {
                Id = contentRef.Id,
                Kind = contentRef.Kind,
                Locator = contentRef.Locator,
                Label = contentRef.Label,
                Scope = contentRef.Scope,
                TenantId = contentRef.TenantId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}