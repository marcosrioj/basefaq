using BaseFaq.Faq.Persistence.FaqDb.Repositories;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.Business.Faq.Queries.GetFaqItemList;

public class FaqItemsGetFaqItemListQueryHandler(IFaqItemRepository faqItemRepository)
    : IRequestHandler<FaqItemsGetFaqItemListQuery, PagedResultDto<FaqItemDto>>
{
    public async Task<PagedResultDto<FaqItemDto>> Handle(
        FaqItemsGetFaqItemListQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Request);

        var query = faqItemRepository.Query(tracking: false);
        query = ApplySorting(query, request.Request.Sorting);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Request.SkipCount)
            .Take(request.Request.MaxResultCount)
            .Select(item => new FaqItemDto
            {
                Id = item.Id,
                Question = item.Question,
                Answer = item.Answer,
                Origin = item.Origin,
                Sort = item.Sort,
                VoteScore = item.VoteScore,
                AiConfidenceScore = item.AiConfidenceScore,
                IsActive = item.IsActive,
                FaqId = item.FaqId
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<FaqItemDto>(totalCount, items);
    }

    private static IQueryable<Persistence.FaqDb.Entities.FaqItem> ApplySorting(
        IQueryable<Persistence.FaqDb.Entities.FaqItem> query, string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting))
        {
            return query.OrderBy(item => item.Question);
        }

        IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>? orderedQuery = null;
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

        return orderedQuery ?? query.OrderBy(item => item.Question);
    }

    private static IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem> ApplyOrder(
        IQueryable<Persistence.FaqDb.Entities.FaqItem> query,
        string fieldName,
        bool desc,
        bool isFirst)
    {
        return fieldName.ToLowerInvariant() switch
        {
            "question" => isFirst
                ? (desc ? query.OrderByDescending(item => item.Question) : query.OrderBy(item => item.Question))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query)
                    .ThenByDescending(item => item.Question)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query).ThenBy(item => item.Question)),
            "answer" => isFirst
                ? (desc ? query.OrderByDescending(item => item.Answer) : query.OrderBy(item => item.Answer))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query)
                    .ThenByDescending(item => item.Answer)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query).ThenBy(item => item.Answer)),
            "origin" => isFirst
                ? (desc ? query.OrderByDescending(item => item.Origin) : query.OrderBy(item => item.Origin))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query)
                    .ThenByDescending(item => item.Origin)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query).ThenBy(item => item.Origin)),
            "sort" => isFirst
                ? (desc ? query.OrderByDescending(item => item.Sort) : query.OrderBy(item => item.Sort))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query)
                    .ThenByDescending(item => item.Sort)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query).ThenBy(item => item.Sort)),
            "votescore" => isFirst
                ? (desc ? query.OrderByDescending(item => item.VoteScore) : query.OrderBy(item => item.VoteScore))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query)
                    .ThenByDescending(item => item.VoteScore)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query).ThenBy(item => item.VoteScore)),
            "aiconfidencescore" => isFirst
                ? (desc
                    ? query.OrderByDescending(item => item.AiConfidenceScore)
                    : query.OrderBy(item => item.AiConfidenceScore))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query)
                    .ThenByDescending(item => item.AiConfidenceScore)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query)
                    .ThenBy(item => item.AiConfidenceScore)),
            "isactive" => isFirst
                ? (desc ? query.OrderByDescending(item => item.IsActive) : query.OrderBy(item => item.IsActive))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query)
                    .ThenByDescending(item => item.IsActive)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query).ThenBy(item => item.IsActive)),
            "faqid" => isFirst
                ? (desc ? query.OrderByDescending(item => item.FaqId) : query.OrderBy(item => item.FaqId))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query)
                    .ThenByDescending(item => item.FaqId)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query).ThenBy(item => item.FaqId)),
            "createddate" => isFirst
                ? (desc ? query.OrderByDescending(item => item.CreatedDate) : query.OrderBy(item => item.CreatedDate))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query)
                    .ThenByDescending(item => item.CreatedDate)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query)
                    .ThenBy(item => item.CreatedDate)),
            "updateddate" => isFirst
                ? (desc ? query.OrderByDescending(item => item.UpdatedDate) : query.OrderBy(item => item.UpdatedDate))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query)
                    .ThenByDescending(item => item.UpdatedDate)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query)
                    .ThenBy(item => item.UpdatedDate)),
            "id" => isFirst
                ? (desc ? query.OrderByDescending(item => item.Id) : query.OrderBy(item => item.Id))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query)
                    .ThenByDescending(item => item.Id)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query).ThenBy(item => item.Id)),
            _ => isFirst
                ? query.OrderBy(item => item.Question)
                : ((IOrderedQueryable<Persistence.FaqDb.Entities.FaqItem>)query).ThenBy(item => item.Question)
        };
    }
}