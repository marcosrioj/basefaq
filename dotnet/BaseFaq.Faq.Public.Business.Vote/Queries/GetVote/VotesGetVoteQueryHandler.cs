using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Models.Faq.Dtos.Vote;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Faq.Public.Business.Vote.Queries.GetVote;

public class VotesGetVoteQueryHandler(FaqDbContext dbContext)
    : IRequestHandler<VotesGetVoteQuery, VoteDto?>
{
    public async Task<VoteDto?> Handle(VotesGetVoteQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Votes
            .AsNoTracking()
            .Where(vote => vote.Id == request.Id)
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