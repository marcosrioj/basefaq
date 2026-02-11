using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.ContentRef;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.FaqWeb.Business.ContentRef.Queries.GetContentRefList;

public class ContentRefsGetContentRefListQueryHandler(FaqDbContext dbContext)
    : IRequestHandler<ContentRefsGetContentRefListQuery, PagedResultDto<ContentRefDto>>
{
    public async Task<PagedResultDto<ContentRefDto>> Handle(
        ContentRefsGetContentRefListQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Request);

        var query = dbContext.ContentRefs.AsNoTracking();
        query = ApplySorting(query, request.Request.Sorting);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Request.SkipCount)
            .Take(request.Request.MaxResultCount)
            .Select(contentRef => new ContentRefDto
            {
                Id = contentRef.Id,
                Kind = contentRef.Kind,
                Locator = contentRef.Locator,
                Label = contentRef.Label,
                Scope = contentRef.Scope,
                TenantId = contentRef.TenantId
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<ContentRefDto>(totalCount, items);
    }

    private static IQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef> ApplySorting(
        IQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef> query,
        string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting))
        {
            return query.OrderByDescending(contentRef => contentRef.UpdatedDate);
        }

        IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>? orderedQuery = null;
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

        return orderedQuery ?? query.OrderByDescending(contentRef => contentRef.UpdatedDate);
    }

    private static IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef> ApplyOrder(
        IQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef> query,
        string fieldName,
        bool desc,
        bool isFirst)
    {
        return fieldName.ToLowerInvariant() switch
        {
            "kind" => isFirst
                ? (desc
                    ? query.OrderByDescending(contentRef => contentRef.Kind)
                    : query.OrderBy(contentRef => contentRef.Kind))
                : (desc
                    ? ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                    .ThenByDescending(contentRef => contentRef.Kind)
                    : ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                    .ThenBy(contentRef => contentRef.Kind)),
            "locator" => isFirst
                ? (desc
                    ? query.OrderByDescending(contentRef => contentRef.Locator)
                    : query.OrderBy(contentRef => contentRef.Locator))
                : (desc
                    ? ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                    .ThenByDescending(contentRef => contentRef.Locator)
                    : ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                    .ThenBy(contentRef => contentRef.Locator)),
            "label" => isFirst
                ? (desc
                    ? query.OrderByDescending(contentRef => contentRef.Label)
                    : query.OrderBy(contentRef => contentRef.Label))
                : (desc
                    ? ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                    .ThenByDescending(contentRef => contentRef.Label)
                    : ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                    .ThenBy(contentRef => contentRef.Label)),
            "scope" => isFirst
                ? (desc
                    ? query.OrderByDescending(contentRef => contentRef.Scope)
                    : query.OrderBy(contentRef => contentRef.Scope))
                : (desc
                    ? ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                    .ThenByDescending(contentRef => contentRef.Scope)
                    : ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                    .ThenBy(contentRef => contentRef.Scope)),
            "createddate" => isFirst
                ? (desc
                    ? query.OrderByDescending(contentRef => contentRef.CreatedDate)
                    : query.OrderBy(contentRef => contentRef.CreatedDate))
                : (desc
                    ? ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                    .ThenByDescending(contentRef => contentRef.CreatedDate)
                    : ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                    .ThenBy(contentRef => contentRef.CreatedDate)),
            "updateddate" => isFirst
                ? (desc
                    ? query.OrderByDescending(contentRef => contentRef.UpdatedDate)
                    : query.OrderBy(contentRef => contentRef.UpdatedDate))
                : (desc
                    ? ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                    .ThenByDescending(contentRef => contentRef.UpdatedDate)
                    : ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                    .ThenBy(contentRef => contentRef.UpdatedDate)),
            "id" => isFirst
                ? (desc
                    ? query.OrderByDescending(contentRef => contentRef.Id)
                    : query.OrderBy(contentRef => contentRef.Id))
                : (desc
                    ? ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                    .ThenByDescending(contentRef => contentRef.Id)
                    : ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                    .ThenBy(contentRef => contentRef.Id)),
            _ => isFirst
                ? query.OrderByDescending(contentRef => contentRef.UpdatedDate)
                : ((IOrderedQueryable<BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities.ContentRef>)query)
                .ThenByDescending(contentRef => contentRef.UpdatedDate)
        };
    }
}