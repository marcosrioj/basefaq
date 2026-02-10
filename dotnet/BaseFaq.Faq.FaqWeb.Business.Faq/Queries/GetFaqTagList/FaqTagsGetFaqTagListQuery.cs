using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqTag;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqTagList;

public class FaqTagsGetFaqTagListQuery : IRequest<PagedResultDto<FaqTagDto>>
{
    public required FaqTagGetAllRequestDto Request { get; set; }
}