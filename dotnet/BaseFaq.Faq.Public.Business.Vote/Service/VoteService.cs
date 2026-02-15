using BaseFaq.Faq.Public.Business.Vote.Abstractions;
using BaseFaq.Faq.Public.Business.Vote.Commands.CreateVote;
using MediatR;
using BaseFaq.Models.Faq.Dtos.Vote;

namespace BaseFaq.Faq.Public.Business.Vote.Service;

public class VoteService(IMediator mediator) : IVoteService
{
    public async Task<Guid> Vote(VoteCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new VotesCreateVoteCommand
        {
            Like = requestDto.Like,
            UnLikeReason = requestDto.UnLikeReason,
            FaqItemId = requestDto.FaqItemId
        };

        return await mediator.Send(command, token);
    }
}