using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Constants;
using BaseFaq.Models.Faq.Dtos.Vote;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.Public.Business.Vote.Queries.GetVote;

public class VotesGetVoteQueryHandler(
    FaqDbContext dbContext,
    IClientKeyContextService clientKeyContextService,
    ITenantClientKeyResolver tenantClientKeyResolver,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<VotesGetVoteQuery, VoteDto?>
{
    public async Task<VoteDto?> Handle(VotesGetVoteQuery request, CancellationToken cancellationToken)
    {
        var clientKey = clientKeyContextService.GetRequiredClientKey();
        var tenantId = await tenantClientKeyResolver.ResolveTenantId(clientKey, cancellationToken);
        httpContextAccessor.HttpContext?.Items[TenantContextKeys.TenantIdItemKey] = tenantId;

        return await dbContext.Votes
            .AsNoTracking()
            .Where(vote => vote.TenantId == tenantId && vote.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}