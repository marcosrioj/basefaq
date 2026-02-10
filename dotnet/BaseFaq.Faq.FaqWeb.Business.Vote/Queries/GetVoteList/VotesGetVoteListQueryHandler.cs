using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.Vote;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.FaqWeb.Business.Vote.Queries.GetVoteList;

public class VotesGetVoteListQueryHandler(FaqDbContext dbContext)
    : IRequestHandler<VotesGetVoteListQuery, PagedResultDto<VoteDto>>
{
    public async Task<PagedResultDto<VoteDto>> Handle(
        VotesGetVoteListQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Request);

        var query = dbContext.Votes.AsNoTracking();
        query = ApplySorting(query, request.Request.Sorting);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Request.SkipCount)
            .Take(request.Request.MaxResultCount)
            .Select(vote => new VoteDto
            {
                Id = vote.Id,
                Like = vote.Like,
                UserPrint = vote.UserPrint,
                Ip = vote.Ip,
                UserAgent = vote.UserAgent,
                UnLikeReason = vote.UnLikeReason,
                FaqItemId = vote.FaqItemId
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<VoteDto>(totalCount, items);
    }

    private static IQueryable<Persistence.FaqDb.Entities.Vote> ApplySorting(
        IQueryable<Persistence.FaqDb.Entities.Vote> query, string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting))
        {
            return query.OrderBy(vote => vote.CreatedDate);
        }

        IOrderedQueryable<Persistence.FaqDb.Entities.Vote>? orderedQuery = null;
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

        return orderedQuery ?? query.OrderBy(vote => vote.CreatedDate);
    }

    private static IOrderedQueryable<Persistence.FaqDb.Entities.Vote> ApplyOrder(
        IQueryable<Persistence.FaqDb.Entities.Vote> query,
        string fieldName,
        bool desc,
        bool isFirst)
    {
        return fieldName.ToLowerInvariant() switch
        {
            "like" => isFirst
                ? (desc ? query.OrderByDescending(vote => vote.Like) : query.OrderBy(vote => vote.Like))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenByDescending(vote => vote.Like)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenBy(vote => vote.Like)),
            "userprint" => isFirst
                ? (desc ? query.OrderByDescending(vote => vote.UserPrint) : query.OrderBy(vote => vote.UserPrint))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenByDescending(vote =>
                        vote.UserPrint)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenBy(vote => vote.UserPrint)),
            "ip" => isFirst
                ? (desc ? query.OrderByDescending(vote => vote.Ip) : query.OrderBy(vote => vote.Ip))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenByDescending(vote => vote.Ip)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenBy(vote => vote.Ip)),
            "useragent" => isFirst
                ? (desc ? query.OrderByDescending(vote => vote.UserAgent) : query.OrderBy(vote => vote.UserAgent))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenByDescending(vote =>
                        vote.UserAgent)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenBy(vote => vote.UserAgent)),
            "unlikereason" => isFirst
                ? (desc ? query.OrderByDescending(vote => vote.UnLikeReason) : query.OrderBy(vote => vote.UnLikeReason))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenByDescending(vote =>
                        vote.UnLikeReason)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenBy(vote => vote.UnLikeReason)),
            "faqitemid" => isFirst
                ? (desc ? query.OrderByDescending(vote => vote.FaqItemId) : query.OrderBy(vote => vote.FaqItemId))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenByDescending(vote =>
                        vote.FaqItemId)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenBy(vote => vote.FaqItemId)),
            "createddate" => isFirst
                ? (desc ? query.OrderByDescending(vote => vote.CreatedDate) : query.OrderBy(vote => vote.CreatedDate))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenByDescending(vote =>
                        vote.CreatedDate)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenBy(vote => vote.CreatedDate)),
            "updateddate" => isFirst
                ? (desc ? query.OrderByDescending(vote => vote.UpdatedDate) : query.OrderBy(vote => vote.UpdatedDate))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenByDescending(vote =>
                        vote.UpdatedDate)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenBy(vote => vote.UpdatedDate)),
            "id" => isFirst
                ? (desc ? query.OrderByDescending(vote => vote.Id) : query.OrderBy(vote => vote.Id))
                : (desc
                    ? ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenByDescending(vote => vote.Id)
                    : ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenBy(vote => vote.Id)),
            _ => isFirst
                ? query.OrderBy(vote => vote.CreatedDate)
                : ((IOrderedQueryable<Persistence.FaqDb.Entities.Vote>)query).ThenBy(vote => vote.CreatedDate)
        };
    }
}