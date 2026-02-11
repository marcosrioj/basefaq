using MediatR;

namespace BaseFaq.Faq.Portal.Business.Vote.Commands.DeleteVote;

public class VotesDeleteVoteCommand : IRequest
{
    public required Guid Id { get; set; }
}