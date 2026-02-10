using BaseFaq.Models.Faq.Enums;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Vote.Commands.CreateVote;

public class VotesCreateVoteCommand : IRequest<Guid>
{
    public required bool Like { get; set; }
    public required string UserPrint { get; set; }
    public required string Ip { get; set; }
    public required string UserAgent { get; set; }
    public UnLikeReason? UnLikeReason { get; set; }
    public required Guid FaqItemId { get; set; }
    public required Guid TenantId { get; set; }
}