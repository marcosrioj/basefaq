using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Repositories;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqItemList;

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

    private static IQueryable<FaqItem> ApplySorting(
        IQueryable<FaqItem> query, string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting))
        {
            return query.OrderBy(item => item.Question);
        }

        IOrderedQueryable<FaqItem>? orderedQuery = null;
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

    private static IOrderedQueryable<FaqItem> ApplyOrder(
        IQueryable<FaqItem> query,
        string fieldName,
        bool desc,
        bool isFirst)
    {
        return fieldName.ToLowerInvariant() switch
        {
            "question" => isFirst
                ? (desc ? query.OrderByDescending(item => item.Question) : query.OrderBy(item => item.Question))
                : (desc
                    ? ((IOrderedQueryable<FaqItem>)query)
                    .ThenByDescending(item => item.Question)
                    : ((IOrderedQueryable<FaqItem>)query).ThenBy(item => item.Question)),
            "answer" => isFirst
                ? (desc ? query.OrderByDescending(item => item.Answer) : query.OrderBy(item => item.Answer))
                : (desc
                    ? ((IOrderedQueryable<FaqItem>)query)
                    .ThenByDescending(item => item.Answer)
                    : ((IOrderedQueryable<FaqItem>)query).ThenBy(item => item.Answer)),
            "origin" => isFirst
                ? (desc ? query.OrderByDescending(item => item.Origin) : query.OrderBy(item => item.Origin))
                : (desc
                    ? ((IOrderedQueryable<FaqItem>)query)
                    .ThenByDescending(item => item.Origin)
                    : ((IOrderedQueryable<FaqItem>)query).ThenBy(item => item.Origin)),
            "sort" => isFirst
                ? (desc ? query.OrderByDescending(item => item.Sort) : query.OrderBy(item => item.Sort))
                : (desc
                    ? ((IOrderedQueryable<FaqItem>)query)
                    .ThenByDescending(item => item.Sort)
                    : ((IOrderedQueryable<FaqItem>)query).ThenBy(item => item.Sort)),
            "votescore" => isFirst
                ? (desc ? query.OrderByDescending(item => item.VoteScore) : query.OrderBy(item => item.VoteScore))
                : (desc
                    ? ((IOrderedQueryable<FaqItem>)query)
                    .ThenByDescending(item => item.VoteScore)
                    : ((IOrderedQueryable<FaqItem>)query).ThenBy(item => item.VoteScore)),
            "aiconfidencescore" => isFirst
                ? (desc
                    ? query.OrderByDescending(item => item.AiConfidenceScore)
                    : query.OrderBy(item => item.AiConfidenceScore))
                : (desc
                    ? ((IOrderedQueryable<FaqItem>)query)
                    .ThenByDescending(item => item.AiConfidenceScore)
                    : ((IOrderedQueryable<FaqItem>)query)
                    .ThenBy(item => item.AiConfidenceScore)),
            "isactive" => isFirst
                ? (desc ? query.OrderByDescending(item => item.IsActive) : query.OrderBy(item => item.IsActive))
                : (desc
                    ? ((IOrderedQueryable<FaqItem>)query)
                    .ThenByDescending(item => item.IsActive)
                    : ((IOrderedQueryable<FaqItem>)query).ThenBy(item => item.IsActive)),
            "faqid" => isFirst
                ? (desc ? query.OrderByDescending(item => item.FaqId) : query.OrderBy(item => item.FaqId))
                : (desc
                    ? ((IOrderedQueryable<FaqItem>)query)
                    .ThenByDescending(item => item.FaqId)
                    : ((IOrderedQueryable<FaqItem>)query).ThenBy(item => item.FaqId)),
            "createddate" => isFirst
                ? (desc ? query.OrderByDescending(item => item.CreatedDate) : query.OrderBy(item => item.CreatedDate))
                : (desc
                    ? ((IOrderedQueryable<FaqItem>)query)
                    .ThenByDescending(item => item.CreatedDate)
                    : ((IOrderedQueryable<FaqItem>)query)
                    .ThenBy(item => item.CreatedDate)),
            "updateddate" => isFirst
                ? (desc ? query.OrderByDescending(item => item.UpdatedDate) : query.OrderBy(item => item.UpdatedDate))
                : (desc
                    ? ((IOrderedQueryable<FaqItem>)query)
                    .ThenByDescending(item => item.UpdatedDate)
                    : ((IOrderedQueryable<FaqItem>)query)
                    .ThenBy(item => item.UpdatedDate)),
            "id" => isFirst
                ? (desc ? query.OrderByDescending(item => item.Id) : query.OrderBy(item => item.Id))
                : (desc
                    ? ((IOrderedQueryable<FaqItem>)query)
                    .ThenByDescending(item => item.Id)
                    : ((IOrderedQueryable<FaqItem>)query).ThenBy(item => item.Id)),
            _ => isFirst
                ? query.OrderBy(item => item.Question)
                : ((IOrderedQueryable<FaqItem>)query).ThenBy(item => item.Question)
        };
    }
}