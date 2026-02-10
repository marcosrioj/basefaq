using BaseFaq.Models.Faq.Enums;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Vote.Commands.UpdateVote;

public class VotesUpdateVoteCommand : IRequest
{
    public required Guid Id { get; set; }
    public required bool Like { get; set; }
    public required string UserPrint { get; set; }
    public required string Ip { get; set; }
    public required string UserAgent { get; set; }
    public UnLikeReason? UnLikeReason { get; set; }
    public required Guid FaqItemId { get; set; }
}