using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Net;
using System.Text;

namespace BaseFaq.Faq.FaqWeb.Business.Vote.Commands.CreateVote;

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

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            throw new ApiErrorException(
                "HttpContext is missing from the current request.",
                errorCode: (int)HttpStatusCode.Unauthorized);
        }

        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        var ip = string.IsNullOrWhiteSpace(forwardedFor)
            ? httpContext.Connection.RemoteIpAddress?.ToString()
            : forwardedFor.Split(',')[0].Trim();
        ip ??= string.Empty;
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        var userPrint = httpContext.User?.Identity?.IsAuthenticated == true
            ? sessionService.GetUserId().ToString()
            : ComputeUserPrint(ip, userAgent);

        var vote = new Persistence.FaqDb.Entities.Vote
        {
            Like = request.Like,
            UserPrint = userPrint,
            Ip = ip,
            UserAgent = userAgent,
            UnLikeReason = request.UnLikeReason,
            FaqItemId = request.FaqItemId
        };

        await dbContext.Votes.AddAsync(vote, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return vote.Id;
    }

    private static string ComputeUserPrint(string ip, string userAgent)
    {
        using var sha = SHA256.Create();
        var payload = $"{ip}|{userAgent}";
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var builder = new StringBuilder(hash.Length * 2);
        foreach (var item in hash)
        {
            builder.Append(item.ToString("x2"));
        }

        return builder.ToString();
    }
}