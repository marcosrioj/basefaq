using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqContentRef;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Abstractions;

public interface IFaqContentRefService
{
    Task<FaqContentRefDto> Create(FaqContentRefCreateRequestDto dto, CancellationToken token);
    Task<PagedResultDto<FaqContentRefDto>> GetAll(FaqContentRefGetAllRequestDto requestDto, CancellationToken token);
    Task<FaqContentRefDto> GetById(Guid id, CancellationToken token);
    Task<FaqContentRefDto> Update(Guid id, FaqContentRefUpdateRequestDto dto, CancellationToken token);
}