using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.Tag;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.FaqWeb.Business.Tag.Queries.GetTagList;

public class TagsGetTagListQueryHandler(FaqDbContext dbContext)
    : IRequestHandler<TagsGetTagListQuery, PagedResultDto<TagDto>>
{
    public async Task<PagedResultDto<TagDto>> Handle(TagsGetTagListQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Request);

        var query = dbContext.Tags.AsNoTracking();
        query = ApplySorting(query, request.Request.Sorting);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Request.SkipCount)
            .Take(request.Request.MaxResultCount)
            .Select(tag => new TagDto
            {
                Id = tag.Id,
                Value = tag.Value,
                TenantId = tag.TenantId
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<TagDto>(totalCount, items);
    }

    private static IQueryable<Persistence.FaqDb.Entities.Tag> ApplySorting(
        IQueryable<Persistence.FaqDb.Entities.Tag> query, string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting))
        {
            return query.OrderBy(tag => tag.Value);
        }

        IOrderedQueryable<Persistence.FaqDb.Entities.Tag>? orderedQuery = null;
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

        return orderedQuery ?? query.OrderBy(tag => tag.Value);
    }

    private static IOrderedQueryable<Persistence.FaqDb.Entities.Tag> ApplyOrder(
        IQueryable<Persistence.FaqDb.Entities.Tag> query,
        string fieldName,
        bool desc,
        bool isFirst)
    {
        return fieldName.ToLowerInvariant() switch
        {
            "value" => isFirst
                ? (desc ? query.OrderByDescending(tag => tag.Value) : query.OrderBy(tag => tag.Value))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.Tag>)query).ThenByDescending(tag => tag.Value)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.Tag>)query).ThenBy(tag => tag.Value)),
            "createddate" => isFirst
                ? (desc ? query.OrderByDescending(tag => tag.CreatedDate) : query.OrderBy(tag => tag.CreatedDate))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.Tag>)query)
                    .ThenByDescending(tag => tag.CreatedDate)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.Tag>)query).ThenBy(tag => tag.CreatedDate)),
            "updateddate" => isFirst
                ? (desc ? query.OrderByDescending(tag => tag.UpdatedDate) : query.OrderBy(tag => tag.UpdatedDate))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.Tag>)query)
                    .ThenByDescending(tag => tag.UpdatedDate)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.Tag>)query).ThenBy(tag => tag.UpdatedDate)),
            "id" => isFirst
                ? (desc ? query.OrderByDescending(tag => tag.Id) : query.OrderBy(tag => tag.Id))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.Tag>)query).ThenByDescending(tag => tag.Id)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.Tag>)query).ThenBy(tag => tag.Id)),
            _ => isFirst
                ? query.OrderBy(tag => tag.Value)
                : ((IOrderedQueryable<Persistence.FaqDb.Entities.Tag>)query).ThenBy(tag => tag.Value)
        };
    }
}