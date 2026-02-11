using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using MediatR;
using System.Net;
using BaseFaq.Faq.Portal.Business.FaqItem.Abstractions;
using BaseFaq.Faq.Portal.Business.FaqItem.Commands.CreateFaqItem;
using BaseFaq.Faq.Portal.Business.FaqItem.Commands.DeleteFaqItem;
using BaseFaq.Faq.Portal.Business.FaqItem.Commands.UpdateFaqItem;
using BaseFaq.Faq.Portal.Business.FaqItem.Queries.GetFaqItem;
using BaseFaq.Faq.Portal.Business.FaqItem.Queries.GetFaqItemList;

namespace BaseFaq.Faq.Portal.Business.FaqItem.Service;

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

    public Task<PagedResultDto<FaqItemDto>> GetAll(FaqItemGetAllRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new FaqItemsGetFaqItemListQuery { Request = requestDto }, token);
    }

    public Task Delete(Guid id, CancellationToken token)
    {
        return mediator.Send(new FaqItemsDeleteFaqItemCommand { Id = id }, token);
    }

    public async Task<FaqItemDto> GetById(Guid id, CancellationToken token)
    {
        var result = await mediator.Send(new FaqItemsGetFaqItemQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"FAQ item '{id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        return result;
    }

    public async Task<FaqItemDto> Update(Guid id, FaqItemUpdateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new FaqItemsUpdateFaqItemCommand
        {
            Id = id,
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

        await mediator.Send(command, token);

        return await GetById(id, token);
    }
}