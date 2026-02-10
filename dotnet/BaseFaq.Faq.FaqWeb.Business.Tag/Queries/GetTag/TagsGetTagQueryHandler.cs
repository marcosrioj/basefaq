using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Models.Faq.Dtos.Tag;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.FaqWeb.Business.Tag.Queries.GetTag;

public class TagsGetTagQueryHandler(FaqDbContext dbContext) : IRequestHandler<TagsGetTagQuery, TagDto?>
{
    public async Task<TagDto?> Handle(TagsGetTagQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Tags
            .AsNoTracking()
            .Where(entity => entity.Id == request.Id)
            .Select(tag => new TagDto
            {
                Id = tag.Id,
                Value = tag.Value,
                TenantId = tag.TenantId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}