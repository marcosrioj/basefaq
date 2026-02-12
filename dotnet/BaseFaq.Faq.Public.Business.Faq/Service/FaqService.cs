using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.Public.Business.Faq.Abstractions;
using BaseFaq.Faq.Public.Business.Faq.Queries.GetFaq;
using BaseFaq.Faq.Public.Business.Faq.Queries.GetFaqList;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.Faq;
using MediatR;
using System.Net;

namespace BaseFaq.Faq.Public.Business.Faq.Service;

public class FaqService(IMediator mediator) : IFaqService
{
    public Task<PagedResultDto<FaqDetailDto>> GetAll(FaqGetAllRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new FaqsGetFaqListQuery { Request = requestDto }, token);
    }

    public async Task<FaqDetailDto> GetById(Guid id, FaqGetRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var result = await mediator.Send(new FaqsGetFaqQuery { Id = id, Request = requestDto }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"FAQ '{id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        return result;
    }
}