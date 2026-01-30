using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.Faq;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqList;

public class FaqsGetFaqListQuery : IRequest<PagedResultDto<FaqDto>>
{
    public required FaqGetAllRequestDto Request { get; set; }
}