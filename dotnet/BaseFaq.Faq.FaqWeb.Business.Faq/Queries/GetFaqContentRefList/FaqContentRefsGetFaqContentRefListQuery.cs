using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqContentRef;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqContentRefList;

public class FaqContentRefsGetFaqContentRefListQuery : IRequest<PagedResultDto<FaqContentRefDto>>
{
    public required FaqContentRefGetAllRequestDto Request { get; set; }
}