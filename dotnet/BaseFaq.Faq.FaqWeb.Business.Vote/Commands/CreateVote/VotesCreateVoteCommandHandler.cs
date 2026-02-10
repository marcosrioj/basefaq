using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Vote.Commands.CreateVote;

public class VotesCreateVoteCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<VotesCreateVoteCommand, Guid>
{
    public async Task<Guid> Handle(VotesCreateVoteCommand request, CancellationToken cancellationToken)
    {
        var vote = new Persistence.FaqDb.Entities.Vote
        {
            Like = request.Like,
            UserPrint = request.UserPrint,
            Ip = request.Ip,
            UserAgent = request.UserAgent,
            UnLikeReason = request.UnLikeReason,
            FaqItemId = request.FaqItemId,
            TenantId = request.TenantId
        };

        await dbContext.Votes.AddAsync(vote, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return vote.Id;
    }
}