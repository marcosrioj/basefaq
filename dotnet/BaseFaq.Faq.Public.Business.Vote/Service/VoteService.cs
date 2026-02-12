using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.Public.Business.Vote.Abstractions;
using BaseFaq.Faq.Public.Business.Vote.Commands.CreateVote;
using BaseFaq.Faq.Public.Business.Vote.Queries.GetVote;
using BaseFaq.Models.Faq.Dtos.Vote;
using MediatR;
using System.Net;

namespace BaseFaq.Faq.Public.Business.Vote.Service;

public class VoteService(IMediator mediator) : IVoteService
{
    public async Task<VoteDto> Vote(VoteCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new VotesCreateVoteCommand
        {
            Like = requestDto.Like,
            UnLikeReason = requestDto.UnLikeReason,
            FaqItemId = requestDto.FaqItemId
        };

        var id = await mediator.Send(command, token);
        var result = await mediator.Send(new VotesGetVoteQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"Vote '{id}' was not found.",
                errorCode: (int)HttpStatusCode.InternalServerError);
        }

        return result;
    }
}