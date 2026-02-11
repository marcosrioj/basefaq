using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.Vote;

namespace BaseFaq.Faq.Portal.Business.Vote.Abstractions;

public interface IVoteService
{
    Task<VoteDto> Create(VoteCreateRequestDto dto, CancellationToken token);
    Task Delete(Guid id, CancellationToken token);
    Task<PagedResultDto<VoteDto>> GetAll(VoteGetAllRequestDto requestDto, CancellationToken token);
    Task<VoteDto> GetById(Guid id, CancellationToken token);
    Task<VoteDto> Update(Guid id, VoteUpdateRequestDto dto, CancellationToken token);
}