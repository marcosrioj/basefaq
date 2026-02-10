using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.Faq;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqList;

public class FaqsGetFaqListQueryHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqsGetFaqListQuery, PagedResultDto<FaqDto>>
{
    public async Task<PagedResultDto<FaqDto>> Handle(FaqsGetFaqListQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Request);

        var query = dbContext.Faqs.AsNoTracking();
        query = ApplySorting(query, request.Request.Sorting);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Request.SkipCount)
            .Take(request.Request.MaxResultCount)
            .Select(faq => new FaqDto
            {
                Id = faq.Id,
                Name = faq.Name,
                Language = faq.Language,
                Status = faq.Status,
                SortStrategy = faq.SortStrategy,
                CtaEnabled = faq.CtaEnabled,
                CtaTarget = faq.CtaTarget,
                TagIds = faq.Tags.Select(tag => tag.TagId).ToList(),
                ContentRefIds = faq.ContentRefs.Select(contentRef => contentRef.ContentRefId).ToList(),
                TenantId = faq.TenantId
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<FaqDto>(totalCount, items);
    }

    private static IQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq> ApplySorting(
        IQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq> query, string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting))
        {
            return query.OrderBy(faq => faq.Name);
        }

        IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>? orderedQuery = null;
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

        return orderedQuery ?? query.OrderBy(faq => faq.Name);
    }

    private static IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq> ApplyOrder(
        IQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq> query,
        string fieldName,
        bool desc,
        bool isFirst)
    {
        return fieldName.ToLowerInvariant() switch
        {
            "name" => isFirst
                ? (desc ? query.OrderByDescending(faq => faq.Name) : query.OrderBy(faq => faq.Name))
                : (desc
                    ? ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query)
                    .ThenByDescending(faq => faq.Name)
                    : ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query).ThenBy(faq => faq.Name)),
            "language" => isFirst
                ? (desc ? query.OrderByDescending(faq => faq.Language) : query.OrderBy(faq => faq.Language))
                : (desc
                    ? ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query).ThenByDescending(faq =>
                        faq.Language)
                    : ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query).ThenBy(faq => faq.Language)),
            "status" => isFirst
                ? (desc ? query.OrderByDescending(faq => faq.Status) : query.OrderBy(faq => faq.Status))
                : (desc
                    ? ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query).ThenByDescending(faq =>
                        faq.Status)
                    : ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query).ThenBy(faq => faq.Status)),
            "sortstrategy" => isFirst
                ? (desc ? query.OrderByDescending(faq => faq.SortStrategy) : query.OrderBy(faq => faq.SortStrategy))
                : (desc
                    ? ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query).ThenByDescending(faq =>
                        faq.SortStrategy)
                    : ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query).ThenBy(faq =>
                        faq.SortStrategy)),
            "ctaenabled" => isFirst
                ? (desc ? query.OrderByDescending(faq => faq.CtaEnabled) : query.OrderBy(faq => faq.CtaEnabled))
                : (desc
                    ? ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query)
                    .ThenByDescending(faq => faq.CtaEnabled)
                    : ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query).ThenBy(faq => faq.CtaEnabled)),
            "ctatarget" => isFirst
                ? (desc ? query.OrderByDescending(faq => faq.CtaTarget) : query.OrderBy(faq => faq.CtaTarget))
                : (desc
                    ? ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query)
                    .ThenByDescending(faq => faq.CtaTarget)
                    : ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query).ThenBy(faq => faq.CtaTarget)),
            "createddate" => isFirst
                ? (desc ? query.OrderByDescending(faq => faq.CreatedDate) : query.OrderBy(faq => faq.CreatedDate))
                : (desc
                    ? ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query)
                    .ThenByDescending(faq => faq.CreatedDate)
                    : ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query).ThenBy(faq => faq.CreatedDate)),
            "updateddate" => isFirst
                ? (desc ? query.OrderByDescending(faq => faq.UpdatedDate) : query.OrderBy(faq => faq.UpdatedDate))
                : (desc
                    ? ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query)
                    .ThenByDescending(faq => faq.UpdatedDate)
                    : ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query).ThenBy(faq => faq.UpdatedDate)),
            "id" => isFirst
                ? (desc ? query.OrderByDescending(faq => faq.Id) : query.OrderBy(faq => faq.Id))
                : (desc
                    ? ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query).ThenByDescending(faq => faq.Id)
                    : ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query).ThenBy(faq => faq.Id)),
            _ => isFirst
                ? query.OrderBy(faq => faq.Name)
                : ((IOrderedQueryable<FaqWeb.Persistence.FaqDb.Entities.Faq>)query).ThenBy(faq => faq.Name)
        };
    }
}