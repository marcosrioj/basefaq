using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Faq.Public.Business.Vote.Helpers;
using BaseFaq.Models.Common.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.Public.Business.Vote.Commands.CreateVote;

public class VotesCreateVoteCommandHandler(
    FaqDbContext dbContext,
    ISessionService sessionService,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<VotesCreateVoteCommand, Guid>
{
    public async Task<Guid> Handle(VotesCreateVoteCommand request, CancellationToken cancellationToken)
    {
        if (!request.Like && request.UnLikeReason is null)
        {
            throw new ApiErrorException(
                "UnLikeReason is required when Like is false.",
                errorCode: (int)HttpStatusCode.UnprocessableEntity);
        }

        var identity = VoteRequestContext.GetIdentity(sessionService, httpContextAccessor);
        var tenantId = sessionService.GetTenantId(AppEnum.Faq);

        var existing = await dbContext.Votes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                vote => vote.FaqItemId == request.FaqItemId && vote.UserPrint == identity.UserPrint,
                cancellationToken);

        if (existing is not null)
        {
            return existing.Id;
        }

        var faqItem = await dbContext.FaqItems
            .FirstOrDefaultAsync(item => item.Id == request.FaqItemId, cancellationToken);
        if (faqItem is null)
        {
            throw new ApiErrorException(
                $"FAQ item '{request.FaqItemId}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        var vote = new Common.Persistence.FaqDb.Entities.Vote
        {
            Like = request.Like,
            UserPrint = identity.UserPrint,
            Ip = identity.Ip,
            UserAgent = identity.UserAgent,
            UnLikeReason = request.UnLikeReason,
            TenantId = tenantId,
            FaqItemId = request.FaqItemId
        };

        await dbContext.Votes.AddAsync(vote, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        faqItem.VoteScore = await dbContext.Votes
            .Where(v => v.FaqItemId == request.FaqItemId)
            .SumAsync(v => v.Like ? 1 : -1, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return vote.Id;
    }
}