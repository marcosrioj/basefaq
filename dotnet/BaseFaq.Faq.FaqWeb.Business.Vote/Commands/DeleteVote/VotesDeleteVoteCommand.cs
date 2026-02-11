using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Vote.Commands.DeleteVote;

public class VotesDeleteVoteCommand : IRequest
{
    public required Guid Id { get; set; }
}