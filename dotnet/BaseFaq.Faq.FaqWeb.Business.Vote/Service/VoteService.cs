using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.FaqWeb.Business.Vote.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.Vote.Commands.CreateVote;
using BaseFaq.Faq.FaqWeb.Business.Vote.Commands.UpdateVote;
using BaseFaq.Faq.FaqWeb.Business.Vote.Queries.GetVote;
using BaseFaq.Faq.FaqWeb.Business.Vote.Queries.GetVoteList;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.Vote;
using MediatR;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Vote.Service;

public class VoteService(IMediator mediator) : IVoteService
{
    public async Task<VoteDto> Create(VoteCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new VotesCreateVoteCommand
        {
            Like = requestDto.Like,
            UserPrint = requestDto.UserPrint,
            Ip = requestDto.Ip,
            UserAgent = requestDto.UserAgent,
            UnLikeReason = requestDto.UnLikeReason,
            FaqItemId = requestDto.FaqItemId
        };

        var id = await mediator.Send(command, token);

        var result = await mediator.Send(new VotesGetVoteQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"Created vote '{id}' was not found.",
                errorCode: (int)HttpStatusCode.InternalServerError);
        }

        return result;
    }

    public Task<PagedResultDto<VoteDto>> GetAll(VoteGetAllRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new VotesGetVoteListQuery { Request = requestDto }, token);
    }

    public async Task<VoteDto> GetById(Guid id, CancellationToken token)
    {
        var result = await mediator.Send(new VotesGetVoteQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"Vote '{id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        return result;
    }

    public async Task<VoteDto> Update(Guid id, VoteUpdateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new VotesUpdateVoteCommand
        {
            Id = id,
            Like = requestDto.Like,
            UserPrint = requestDto.UserPrint,
            Ip = requestDto.Ip,
            UserAgent = requestDto.UserAgent,
            UnLikeReason = requestDto.UnLikeReason,
            FaqItemId = requestDto.FaqItemId
        };

        await mediator.Send(command, token);

        return await GetById(id, token);
    }
}