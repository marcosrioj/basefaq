using BaseFaq.Faq.Business.Faq.Abstractions;
using BaseFaq.Faq.Business.Faq.Commands.CreateFaqItem;
using BaseFaq.Faq.Business.Faq.Commands.UpdateFaqItem;
using BaseFaq.Faq.Business.Faq.Queries.GetFaqItem;
using BaseFaq.Faq.Business.Faq.Queries.GetFaqItemList;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using MediatR;

namespace BaseFaq.Faq.Business.Faq.Service;

public class FaqItemService(IMediator mediator) : IFaqItemService
{
    public async Task<FaqItemDto> Create(FaqItemCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new FaqItemsCreateFaqItemCommand
        {
            Question = requestDto.Question,
            Answer = requestDto.Answer,
            Origin = requestDto.Origin,
            Sort = requestDto.Sort,
            VoteScore = requestDto.VoteScore,
            AiConfidenceScore = requestDto.AiConfidenceScore,
            IsActive = requestDto.IsActive,
            FaqId = requestDto.FaqId
        };

        var id = await mediator.Send(command, token);

        var result = await mediator.Send(new FaqItemsGetFaqItemQuery { Id = id }, token);
        if (result is null)
        {
            throw new InvalidOperationException($"Created FAQ item '{id}' was not found.");
        }

        return result;
    }

    public Task<PagedResultDto<FaqItemDto>> GetAll(FaqItemGetAllRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new FaqItemsGetFaqItemListQuery { Request = requestDto }, token);
    }

    public async Task<FaqItemDto> GetById(Guid id, CancellationToken token)
    {
        var result = await mediator.Send(new FaqItemsGetFaqItemQuery { Id = id }, token);
        if (result is null)
        {
            throw new KeyNotFoundException($"FAQ item '{id}' was not found.");
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
            Answer = requestDto.Answer,
            Origin = requestDto.Origin,
            Sort = requestDto.Sort,
            VoteScore = requestDto.VoteScore,
            AiConfidenceScore = requestDto.AiConfidenceScore,
            IsActive = requestDto.IsActive,
            FaqId = requestDto.FaqId
        };

        await mediator.Send(command, token);

        return await GetById(id, token);
    }
}