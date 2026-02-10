using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Vote.Commands.UpdateVote;

public class VotesUpdateVoteCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<VotesUpdateVoteCommand>
{
    public async Task Handle(VotesUpdateVoteCommand request, CancellationToken cancellationToken)
    {
        var vote = await dbContext.Votes.FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (vote is null)
        {
            throw new ApiErrorException(
                $"Vote '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        vote.Like = request.Like;
        vote.UserPrint = request.UserPrint;
        vote.Ip = request.Ip;
        vote.UserAgent = request.UserAgent;
        vote.UnLikeReason = request.UnLikeReason;
        vote.FaqItemId = request.FaqItemId;

        dbContext.Votes.Update(vote);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}