using BaseFaq.Models.Faq.Enums;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Vote.Commands.UpdateVote;

public class VotesUpdateVoteCommand : IRequest
{
    public required Guid Id { get; set; }
    public required bool Like { get; set; }
    public UnLikeReason? UnLikeReason { get; set; }
    public required Guid FaqItemId { get; set; }
}