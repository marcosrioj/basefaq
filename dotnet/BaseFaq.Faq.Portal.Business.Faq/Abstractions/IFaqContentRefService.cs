using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqContentRef;

namespace BaseFaq.Faq.Portal.Business.Faq.Abstractions;

public interface IFaqContentRefService
{
    Task<Guid> Create(FaqContentRefCreateRequestDto dto, CancellationToken token);
    Task Delete(Guid id, CancellationToken token);
    Task<PagedResultDto<FaqContentRefDto>> GetAll(FaqContentRefGetAllRequestDto requestDto, CancellationToken token);
    Task<FaqContentRefDto> GetById(Guid id, CancellationToken token);
    Task<Guid> Update(Guid id, FaqContentRefUpdateRequestDto dto, CancellationToken token);
}