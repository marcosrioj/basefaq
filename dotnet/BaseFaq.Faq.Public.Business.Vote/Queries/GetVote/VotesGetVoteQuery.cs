using BaseFaq.Models.Faq.Dtos.Vote;
using MediatR;

namespace BaseFaq.Faq.Public.Business.Vote.Queries.GetVote;

public class VotesGetVoteQuery : IRequest<VoteDto?>
{
    public required Guid Id { get; set; }
}