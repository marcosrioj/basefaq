using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqTag;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqTagList;

public class FaqTagsGetFaqTagListQueryHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqTagsGetFaqTagListQuery, PagedResultDto<FaqTagDto>>
{
    public async Task<PagedResultDto<FaqTagDto>> Handle(
        FaqTagsGetFaqTagListQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Request);

        var query = dbContext.FaqTags.AsNoTracking();
        query = ApplySorting(query, request.Request.Sorting);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Request.SkipCount)
            .Take(request.Request.MaxResultCount)
            .Select(faqTag => new FaqTagDto
            {
                Id = faqTag.Id,
                FaqId = faqTag.FaqId,
                TagId = faqTag.TagId,
                TenantId = faqTag.TenantId
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<FaqTagDto>(totalCount, items);
    }

    private static IQueryable<Persistence.FaqDb.Entities.FaqTag> ApplySorting(
        IQueryable<Persistence.FaqDb.Entities.FaqTag> query,
        string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting))
        {
            return query.OrderByDescending(faqTag => faqTag.UpdatedDate);
        }

        IOrderedQueryable<Persistence.FaqDb.Entities.FaqTag>? orderedQuery = null;
        var fields = sorting.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var field in fields)
        {
            var parts = field.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 0)
            {
                continue;
            }

            var fieldName = parts[0];
            var desc = parts.Length > 1 && parts[1].Equals("DESC", StringComparison.OrdinalIgnoreCase);

            orderedQuery = ApplyOrder(orderedQuery ?? query, fieldName, desc, orderedQuery is null);
        }

        return orderedQuery ?? query.OrderByDescending(faqTag => faqTag.UpdatedDate);
    }

    private static IOrderedQueryable<Persistence.FaqDb.Entities.FaqTag> ApplyOrder(
        IQueryable<Persistence.FaqDb.Entities.FaqTag> query,
        string fieldName,
        bool desc,
        bool isFirst)
    {
        return fieldName.ToLowerInvariant() switch
        {
            "faqid" => isFirst
                ? (desc ? query.OrderByDescending(faqTag => faqTag.FaqId) : query.OrderBy(faqTag => faqTag.FaqId))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqTag>)query)
                    .ThenByDescending(faqTag => faqTag.FaqId)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqTag>)query).ThenBy(faqTag => faqTag.FaqId)),
            "tagid" => isFirst
                ? (desc ? query.OrderByDescending(faqTag => faqTag.TagId) : query.OrderBy(faqTag => faqTag.TagId))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqTag>)query)
                    .ThenByDescending(faqTag => faqTag.TagId)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqTag>)query).ThenBy(faqTag => faqTag.TagId)),
            "createddate" => isFirst
                ? (desc
                    ? query.OrderByDescending(faqTag => faqTag.CreatedDate)
                    : query.OrderBy(faqTag => faqTag.CreatedDate))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqTag>)query)
                    .ThenByDescending(faqTag => faqTag.CreatedDate)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqTag>)query).ThenBy(faqTag =>
                        faqTag.CreatedDate)),
            "updateddate" => isFirst
                ? (desc
                    ? query.OrderByDescending(faqTag => faqTag.UpdatedDate)
                    : query.OrderBy(faqTag => faqTag.UpdatedDate))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqTag>)query)
                    .ThenByDescending(faqTag => faqTag.UpdatedDate)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqTag>)query).ThenBy(faqTag =>
                        faqTag.UpdatedDate)),
            "id" => isFirst
                ? (desc ? query.OrderByDescending(faqTag => faqTag.Id) : query.OrderBy(faqTag => faqTag.Id))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqTag>)query)
                    .ThenByDescending(faqTag => faqTag.Id)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqTag>)query).ThenBy(faqTag => faqTag.Id)),
            _ => isFirst
                ? query.OrderByDescending(faqTag => faqTag.UpdatedDate)
                : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqTag>)query)
                .ThenByDescending(faqTag => faqTag.UpdatedDate)
        };
    }
}