using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.Portal.Business.Vote.Commands.DeleteVote;

public class VotesDeleteVoteCommandHandler(FaqDbContext dbContext) : IRequestHandler<VotesDeleteVoteCommand>
{
    public async Task Handle(VotesDeleteVoteCommand request, CancellationToken cancellationToken)
    {
        var vote = await dbContext.Votes
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (vote is null)
        {
            throw new ApiErrorException(
                $"Vote '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        dbContext.Votes.Remove(vote);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}