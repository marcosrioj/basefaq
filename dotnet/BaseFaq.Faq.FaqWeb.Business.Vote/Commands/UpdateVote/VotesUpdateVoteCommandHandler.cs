using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.Vote.Helpers;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Vote.Commands.UpdateVote;

public class VotesUpdateVoteCommandHandler(
    FaqDbContext dbContext,
    ISessionService sessionService,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<VotesUpdateVoteCommand>
{
    public async Task Handle(VotesUpdateVoteCommand request, CancellationToken cancellationToken)
    {
        if (!request.Like && request.UnLikeReason is null)
        {
            throw new ApiErrorException(
                "UnLikeReason is required when Like is false.",
                errorCode: (int)HttpStatusCode.BadRequest);
        }

        var vote = await dbContext.Votes.FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (vote is null)
        {
            throw new ApiErrorException(
                $"Vote '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        var identity = VoteRequestContext.GetIdentity(sessionService, httpContextAccessor);

        vote.Like = request.Like;
        vote.UserPrint = identity.UserPrint;
        vote.Ip = identity.Ip;
        vote.UserAgent = identity.UserAgent;
        vote.UnLikeReason = request.UnLikeReason;
        vote.FaqItemId = request.FaqItemId;

        dbContext.Votes.Update(vote);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}