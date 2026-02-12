using BaseFaq.Models.Faq.Dtos.Vote;

namespace BaseFaq.Faq.Public.Business.Vote.Abstractions;

public interface IVoteService
{
    Task<VoteDto> Vote(VoteCreateRequestDto requestDto, CancellationToken token);
}