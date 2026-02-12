using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.Public.Business.FaqItem.Abstractions;
using BaseFaq.Faq.Public.Business.FaqItem.Commands.CreateFaqItem;
using BaseFaq.Faq.Public.Business.FaqItem.Queries.GetFaqItem;
using BaseFaq.Faq.Public.Business.FaqItem.Queries.SearchFaqItem;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using MediatR;
using System.Net;

namespace BaseFaq.Faq.Public.Business.FaqItem.Service;

public class FaqItemService(IMediator mediator) : IFaqItemService
{
    public async Task<FaqItemDto> Create(FaqItemCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new FaqItemsCreateFaqItemCommand
        {
            Question = requestDto.Question,
            ShortAnswer = requestDto.ShortAnswer,
            Answer = requestDto.Answer,
            AdditionalInfo = requestDto.AdditionalInfo,
            CtaTitle = requestDto.CtaTitle,
            CtaUrl = requestDto.CtaUrl,
            Sort = requestDto.Sort,
            VoteScore = requestDto.VoteScore,
            AiConfidenceScore = requestDto.AiConfidenceScore,
            IsActive = requestDto.IsActive,
            FaqId = requestDto.FaqId,
            ContentRefId = requestDto.ContentRefId
        };

        var id = await mediator.Send(command, token);
        var result = await mediator.Send(new FaqItemsGetFaqItemQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"Created FAQ item '{id}' was not found.",
                errorCode: (int)HttpStatusCode.InternalServerError);
        }

        return result;
    }

    public Task<PagedResultDto<FaqItemDto>> Search(FaqItemSearchRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new FaqItemsSearchFaqItemQuery { Request = requestDto }, token);
    }
}