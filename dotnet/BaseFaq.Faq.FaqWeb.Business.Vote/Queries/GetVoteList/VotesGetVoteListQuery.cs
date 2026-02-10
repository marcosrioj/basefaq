using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.Vote;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Vote.Queries.GetVoteList;

public class VotesGetVoteListQuery : IRequest<PagedResultDto<VoteDto>>
{
    public required VoteGetAllRequestDto Request { get; set; }
}